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
        private readonly IModeratorManager _moderatorManager;

        public UsersInteractionsController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager, IModeratorManager moderatorManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _moderatorManager = moderatorManager;
        }

        [HttpPost]
        [Authorize(Roles = "employer, moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null || job.IsClosed)
                return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);
            switch (currentUser)
            {
                case Moderator _ when !job.IsPaymentDenied:
                    return BadRequest();
                case Employer employer:
                    if (employer.Id != job.Employer.Id)
                        return BadRequest();
                    break;
            }

            // TODO: Transfer money from platform balance to freelancer balance
            job.IsClosed = true;
            await _freelancingPlatform.JobManager.UpdateAsync(job);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyFreelancersWork(int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null || job.IsClosed)
                return BadRequest();

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
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptDenying(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || job.IsClosed || !job.IsPaymentDenied)
                return BadRequest();

            // TODO: Transfer money from platform balance to employer balance
            job.IsClosed = true;
            await _freelancingPlatform.JobManager.UpdateAsync(job);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefuseDenying(int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return BadRequest();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || job.IsClosed || !job.IsPaymentDenied)
                return BadRequest();

            await _freelancingPlatform.JobManager.UpdateAsync(job);
            var notification = new Notification
            {
                Receiver = await _moderatorManager.GetModeratorAsync(),
                CreatedDate = DateTime.Now,
                Source = Url.Action("JobInfo", "Job", new { jobId }),
                Title = $"{job.Employer} wants to deny a job. Please follow the link and choose an action."
            };

            await _freelancingPlatform.NotificationManager.AddNotificationAsync(notification, job.HiredFreelancer);
            return RedirectToAction("JobBoard", "Job");
        }
    }
}