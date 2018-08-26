using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    public class UsersInteractionsController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public UsersInteractionsController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null)
                return BadRequest();

            // TODO: Transfer money from platform balance to freelancer balance
            await _freelancingPlatform.ProposalManager.DeleteRangeAsync(job.Proposals);
            await _freelancingPlatform.JobManager.DeleteAsync(job);

            return Redirect("JobBoard");
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyFreelancersWork(int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null)
                return BadRequest();
            // TODO: Send an accepting job denying notification to the hired freelancer.
            job.IsPaymentDenied = true;
            await _freelancingPlatform.JobManager.UpdateAsync(job);
            var notification = new Notification
            {
                Receiver = job.HiredFreelancer,
                CreatedDate = DateTime.Now,
                Source = Url.Action("JobInfo", "Job", new { jobId }),
                Title = $"{currentUser.UserName} wants to deny a job. Please follow the link and choose an action."
            };

            await _freelancingPlatform.NotificationManager.AddNotificationAsync(notification, job.HiredFreelancer);
            return Redirect("JobBoard");
        }
    }
}