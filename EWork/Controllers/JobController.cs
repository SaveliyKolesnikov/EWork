using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.Services.Mappers.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    [Authorize(Roles = "employer, freelancer, moderator, administrator")]
    public class JobController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<PhotoConfig> _photoOptions;
        private readonly IJobMapper _jobMapper;
        private readonly int _takeAmount;

        public JobController(IFreelancingPlatform freelancingPlatform,
            UserManager<User> userManager,
            IHostingEnvironment environment,
            IOptions<PhotoConfig> photoOptions,
            IOptions<FreelancingPlatformConfig> fpOptions,
            IJobMapper jobMapper)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _environment = environment;
            _photoOptions = photoOptions;
            _jobMapper = jobMapper;
            _takeAmount = fpOptions.Value.TakeAmount;
        }

        public class FilterModel
        {
            private double _employerRatingTo;
            private double _employerRatingFrom;
            private double _budgetFrom;
            private double _budgetTo;
            public string RequiredTags { get; set; }

            public double EmployerRatingFrom
            {
                get => _employerRatingFrom;
                set => _employerRatingFrom = value < 0.001 ? 0 : value;
            }

            public double EmployerRatingTo
            {
                get => _employerRatingTo;
                set => _employerRatingTo = value < 0.001 ? 10d : value;
            }

            public double BudgetFrom
            {
                get => _budgetFrom;
                set => _budgetFrom = value < 0.001 ? 0 : value;
            }

            public double BudgetTo
            {
                get => _budgetTo;
                set => _budgetTo = value < 0.001 ? double.MaxValue : value;
            }

            public void ValidateValues()
                => (BudgetFrom, BudgetTo, EmployerRatingFrom, EmployerRatingTo) =
                    (BudgetFrom, BudgetTo, EmployerRatingFrom, EmployerRatingTo);
        }

        public async Task<IActionResult> JobBoard(FilterModel filterModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var jobs = GetJobsUsingFilters(filterModel);

            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            jobs = jobs.Take(_takeAmount);
            var searchUrl = Url.Action("JobBoard");
            var ajaxSearchUrl = Url.Action("GetJobsAjax");
            var jobBoardViewModel = new JobBoardViewModel(await jobs.ToArrayAsync(), filterModel, searchUrl, ajaxSearchUrl, _takeAmount, currentUser);
            return View(jobBoardViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetJobsAjax(int skipAmount, int takeAmount, FilterModel filterModel)
        {
            var jobs = GetJobsUsingFilters(filterModel);

            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            jobs = jobs.Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        private IQueryable<Job> GetJobsUsingFilters(FilterModel filterModel)
        {
            filterModel.ValidateValues();
            var jobs = _freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed);

            if (!(filterModel.RequiredTags is null))
            {
                // Tag length cannot be greater than 20.
                var tags = filterModel.RequiredTags.Split(' ').Where(tag => tag.Length <= 20);
                jobs = jobs.Where(j => j.JobTags.Any(jt =>
                    tags.Any(tagText => jt.Tag.Text.Equals(tagText, StringComparison.InvariantCultureIgnoreCase))));
            }

            if (filterModel.BudgetTo < filterModel.BudgetFrom)
                (filterModel.BudgetTo, filterModel.BudgetFrom) = (filterModel.BudgetFrom, filterModel.BudgetTo);
            if (filterModel.EmployerRatingTo < filterModel.EmployerRatingFrom)
                (filterModel.EmployerRatingTo, filterModel.EmployerRatingFrom) = (filterModel.EmployerRatingFrom, filterModel.EmployerRatingTo);

            return jobs
                .Where(j => (j.Employer.Reviews.Count == 0 ? 0d : j.Employer.Reviews.Average(r => r.Value)) >= filterModel.EmployerRatingFrom &&
                            (j.Employer.Reviews.Count == 0 ? 0d : j.Employer.Reviews.Average(r => r.Value)) <= filterModel.EmployerRatingTo)
                .Where(j => (double)j.Budget >= filterModel.BudgetFrom && (double)j.Budget <= filterModel.BudgetTo)
                .OrderByDescending(j => j.CreationDate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetJobsByTitleAjax(int skipAmount, int takeAmount, string title)
        {
            var jobs = _freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed && j.Title.StartsWith(title));

            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            jobs = jobs.Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [HttpPost]
        [Authorize(Roles = "employer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var deletedJob = await _freelancingPlatform.JobManager.FindAsync(job => !job.IsClosed && job.Id == jobId);
            if (deletedJob is null)
                return UnprocessableEntity(jobId);

            if (User.IsInRole("employer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (deletedJob.Employer.Id != currentUser.Id)
                    return Forbid();
            }
            await _freelancingPlatform.JobManager.DeleteAsync(deletedJob);
            return Ok();
        }

        public IActionResult CreateJob() => View();

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(Job job, string tags)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var platformBalance = await _freelancingPlatform.BalanceManager.GetFreelancingPlatformBalanceAsync();
            var currentUserBalance = await _freelancingPlatform.BalanceManager.FindAsync(b => b.UserId == currentUser.Id);

            try
            {
                await _freelancingPlatform.BalanceManager.TransferMoneyAsync(currentUserBalance, platformBalance,
                    job.Budget);
            }
            catch (NotEnoughMoneyException e)
            {
                return Content("Error!" + Environment.NewLine + e.Message);
            }

            job.Employer = currentUser;
            job.Proposals = new List<Proposal>();
            job.JobTags = new List<JobTags>();
            job.CreationDate = DateTime.Now;

            if (!(tags is null))
            {
                var splitTags = tags.Trim().Split(' ').Where(tag => tag.Length <= 20);
                var newTags = await _freelancingPlatform.TagManager.AddTagsRangeAsync(splitTags);
                foreach (var tag in newTags)
                    job.JobTags.Add(new JobTags { Tag = tag });
            }

            await _freelancingPlatform.JobManager.AddAsync(job);
            return Redirect("JobBoard");
        }

        public async Task<IActionResult> JobInfo(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null)
                return UnprocessableEntity(jobId);

            if (job.IsClosed)
                ViewBag.Heading = "Closed Job";

            Proposal proposal = null;
            User currentUser = null;
            if (User.IsInRole("freelancer"))
            {
                if (!(job.HiredFreelancer is null) && job.HiredFreelancer.Id != _userManager.GetUserId(User))
                    return Forbid();

                currentUser = await _userManager.GetUserAsync(User);
                proposal = job.Proposals.Find(p => p.Sender.Id == currentUser.Id);
            }

            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);
            var jobInfoViewModel = new JobInfoViewModel(currentUser, job, proposal, pathToProfilePhotos);
            return View(jobInfoViewModel);
        }



        [Authorize(Roles = "freelancer, moderator, administrator")]
        public async Task<IActionResult> FreelancerContracts(FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return Forbid();

            var jobs = GetJobsUsingFilters(filterModel).Where(j => !j.IsClosed && j.HiredFreelancer.Id == currentUser.Id).Take(_takeAmount);

            var searchUrl = Url.Action("FreelancerContracts");
            var ajaxSearchUrl = Url.Action("FreelancerContractsAjax");
            var jobBoardViewModel = new JobBoardViewModel(await jobs.ToArrayAsync(), filterModel, searchUrl, ajaxSearchUrl, _takeAmount);
            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FreelancerContractsAjax(int skipAmount, int takeAmount, FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.HiredFreelancer.Id == currentUser.Id)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [Authorize(Roles = "employer")]
        public async Task<IActionResult> EmployerOpenedJobs(FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer == null).Take(_takeAmount);

            var searchUrl = Url.Action("EmployerOpenedJobs");
            var ajaxSearchUrl = Url.Action("EmployerOpenedJobsAjax");
            var jobBoardViewModel = new JobBoardViewModel(await jobs.ToArrayAsync(), filterModel, searchUrl, ajaxSearchUrl, _takeAmount, currentUser);

            ViewData["Title"] = "Jobs";
            ViewBag.Heading = "Opened Jobs";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EmployerOpenedJobsAjax(int skipAmount, int takeAmount, FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer == null)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [Authorize(Roles = "employer")]
        public async Task<IActionResult> EmployerContracts(FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer != null).Take(_takeAmount);

            var searchUrl = Url.Action("EmployerContracts");
            var ajaxSearchUrl = Url.Action("EmployerContractsAjax");
            var jobBoardViewModel = new JobBoardViewModel(await jobs.ToArrayAsync(), filterModel, searchUrl, ajaxSearchUrl, _takeAmount, currentUser);

            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EmployerContractsAjax(int skipAmount, int takeAmount, FilterModel filterModel)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer != null)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [Authorize(Roles = "freelancer, moderator, administrator")]
        public async Task<IActionResult> AllFreelancerProposals(FilterModel filterModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Proposals.Any(p => p.Sender.Id == currentUser.Id)).Take(_takeAmount);

            var searchUrl = Url.Action("AllFreelancerProposals");
            var ajaxSearchUrl = Url.Action("AllFreelancerProposalsAjax");
            var jobBoardViewModel = new JobBoardViewModel(await jobs.ToArrayAsync(), filterModel, searchUrl, ajaxSearchUrl, _takeAmount);

            ViewData["Title"] = "Proposals";
            ViewBag.Heading = "Jobs with Your Proposal";
            return View("~/Views/Job/JobBoard.cshtml", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AllFreelancerProposalsAjax(int skipAmount, int takeAmount, FilterModel filterModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var jobs = GetJobsUsingFilters(filterModel).Where(j => j.Proposals.Any(p => p.Sender.Id == currentUser.Id))
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
