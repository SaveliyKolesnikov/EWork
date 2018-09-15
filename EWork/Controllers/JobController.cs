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

        public JobController(IFreelancingPlatform freelancingPlatform,
            UserManager<User> userManager,
            IHostingEnvironment environment,
            IOptions<PhotoConfig> photoOptions,
            IJobMapper jobMapper)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _environment = environment;
            _photoOptions = photoOptions;
            _jobMapper = jobMapper;
        }

        public IActionResult JobBoard(string requiredTags)
        {
            var jobs = GetJobsByTags(requiredTags);
            var usedTags = requiredTags is null ? Enumerable.Empty<string>() : requiredTags.Split(' ').Where(tag => tag.Length <= 20);

            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            var searchUrl = Url.Action("JobBoard");
            var ajaxSearchUrl = Url.Action("GetJobsAjax");
            var jobBoardViewModel = new JobBoardViewModel(jobs, usedTags, searchUrl, ajaxSearchUrl);
            return View(jobBoardViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetJobsAjax(int skipAmount, int takeAmount, string requiredTags)
        {
            var jobs = GetJobsByTags(requiredTags);

            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            jobs = jobs.Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        private IQueryable<Job> GetJobsByTags(string requiredTags)
        {
            var jobs = _freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed);

            if (!(requiredTags is null))
            {
                // Tag length cannot be greater than 20.
                var tags = requiredTags.Split(' ').Where(tag => tag.Length <= 20);
                jobs = jobs.Where(j => j.JobTags.Any(jt =>
                    tags.Any(tagText => jt.Tag.Text.Equals(tagText, StringComparison.InvariantCultureIgnoreCase))));
            }

            return jobs;
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

            var currentUser = await _userManager.GetUserAsync(User);
            if (deletedJob.Employer.Id != currentUser.Id)
                return Forbid();

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
        public async Task<IActionResult> FreelancerContracts(string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return Forbid();

            var jobs = GetJobsByTags(requiredTags).Where(j => !j.IsClosed && j.HiredFreelancer.Id == currentUser.Id);

            var usedTags = requiredTags is null ? Enumerable.Empty<string>() : requiredTags.Split(' ').Where(tag => tag.Length <= 20);


            var searchUrl = Url.Action("FreelancerContracts");
            var ajaxSearchUrl = Url.Action("FreelancerContractsAjax");
            var jobBoardViewModel = new JobBoardViewModel(jobs, usedTags, searchUrl, ajaxSearchUrl);
            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FreelancerContractsAjax(int skipAmount, int takeAmount, string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsByTags(requiredTags).Where(j => j.HiredFreelancer.Id == currentUser.Id)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [Authorize(Roles = "employer, moderator, administrator")]
        public async Task<IActionResult> OpenedJobs(string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Employer.Id == currentUser.Id);

            var usedTags = requiredTags is null ? Enumerable.Empty<string>() : requiredTags.Split(' ').Where(tag => tag.Length <= 20);


            var searchUrl = Url.Action("OpenedJobs");
            var ajaxSearchUrl = Url.Action("OpenedJobsAjax");
            var jobBoardViewModel = new JobBoardViewModel(jobs, usedTags, searchUrl, ajaxSearchUrl);

            ViewData["Title"] = "Jobs";
            ViewBag.Heading = "Opened Jobs";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "employer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OpenedJobsAjax(int skipAmount, int takeAmount, string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Employer.Id == currentUser.Id)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }



        [Authorize(Roles = "employer, moderator, administrator")]
        public async Task<IActionResult> EmployerContracts(string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer != null);

            var usedTags = requiredTags is null ? Enumerable.Empty<string>() : requiredTags.Split(' ').Where(tag => tag.Length <= 20);

            var searchUrl = Url.Action("EmployerContracts");
            var ajaxSearchUrl = Url.Action("EmployerContractsAjax");
            var jobBoardViewModel = new JobBoardViewModel(jobs, usedTags, searchUrl, ajaxSearchUrl);

            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "employer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EmployerContractsAjax(int skipAmount, int takeAmount, string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer != null)
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [Authorize(Roles = "freelancer, moderator, administrator")]
        public async Task<IActionResult> AllFreelancerProposals(string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return Forbid();

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Proposals.Any(p => p.Sender.Id == currentUser.Id));
            var usedTags = requiredTags is null ? Enumerable.Empty<string>() : requiredTags.Split(' ').Where(tag => tag.Length <= 20);


            var searchUrl = Url.Action("AllFreelancerProposals");
            var ajaxSearchUrl = Url.Action("AllFreelancerProposalsAjax");
            var jobBoardViewModel = new JobBoardViewModel(jobs, usedTags, searchUrl, ajaxSearchUrl);
            ViewData["Title"] = "Proposals";
            ViewBag.Heading = "Jobs with Your Proposal";
            return View("~/Views/Job/JobBoard.cshtml", jobBoardViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AllFreelancerProposalsAjax(int skipAmount, int takeAmount, string requiredTags)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var jobs = GetJobsByTags(requiredTags).Where(j => j.Proposals.Any(p => p.Sender.Id == currentUser.Id))
                .Skip(skipAmount).Take(takeAmount);

            return Json(_jobMapper.MapRange(await jobs.ToArrayAsync()));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
