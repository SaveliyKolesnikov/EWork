using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public JobController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        [Authorize(Roles = "employer, freelancer, moderator")]
        public IActionResult JobBoard()
        {
            var jobs = _freelancingPlatform.JobManager.GetAll();
            if (User.IsInRole("freelancer"))
                jobs = jobs.Where(j => j.HiredFreelancer == null);

            return View(jobs);
        }

        [HttpPost]
        [Authorize(Roles = "employer, moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var deletedJob = await _freelancingPlatform.JobManager.FindAsync(job => job.Id == jobId);
            if (deletedJob is null)
                return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);
            if (deletedJob.Employer.Id != currentUser.Id)
                return BadRequest();

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
                return BadRequest();

            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            // TODO: Transfer money from employer balance to platform balance
            job.Employer = currentUser;
            job.Proposals = new List<Proposal>();
            job.JobTags = new List<JobTags>();
            job.CreationDate = DateTime.Now;

            if (!(tags is null))
            {
                var newTags = await _freelancingPlatform.TagManager.AddTagsRangeAsync(tags.Trim().Split(' '));
                foreach (var tag in newTags)
                    job.JobTags.Add(new JobTags { Tag = tag });
            }

            await _freelancingPlatform.JobManager.AddAsync(job);
            return Redirect("JobBoard");
        }

        [HttpGet]
        [Authorize(Roles = "employer, freelancer, moderator")]
        public async Task<IActionResult> JobInfo(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null)
                return BadRequest();

            Proposal proposal = null;
            User currentUser = null;
            if (User.IsInRole("freelancer"))
            {
                currentUser = await _userManager.GetUserAsync(User);
                proposal = job.Proposals.Find(p => p.Sender.Id == currentUser.Id);
            }

            var jobInfoViewModel = new JobInfoViewModel(currentUser, job, proposal);
            return View(jobInfoViewModel);
        }

        

        [Authorize(Roles = "freelancer")]
        public async Task<IActionResult> FreelancerContracts()
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return BadRequest();

            var jobs = _freelancingPlatform.JobManager.GetAll()
                .Where(j => j.HiredFreelancer.Id == currentUser.Id);

            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobs);
        }

        [Authorize(Roles = "employer")]
        public async Task<IActionResult> OpenedJobs()
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var jobs = _freelancingPlatform.JobManager.GetAll()
                .Where(j => j.Employer.Id == currentUser.Id);

            ViewData["Title"] = "Jobs";
            ViewBag.Heading = "Opened Jobs";
            return View("JobBoard", jobs);
        }

        [Authorize(Roles = "employer")]
        public async Task<IActionResult> EmployerContracts()
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var jobs = _freelancingPlatform.JobManager.GetAll()
                .Where(j => j.Employer.Id == currentUser.Id && j.HiredFreelancer != null);

            ViewData["Title"] = "Contracts";
            ViewBag.Heading = "Your Contracts";
            return View("JobBoard", jobs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
