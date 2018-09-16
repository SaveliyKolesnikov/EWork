using System;
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
        [Authorize(Roles = "employer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null || job.IsClosed)
                return UnprocessableEntity(jobId);

            var currentUser = await _userManager.GetUserAsync(User);
            switch (currentUser)
            {
                case Moderator _ when !job.IsPaymentDenied:
                    return Forbid();
                case Employer employer:
                    if (employer.Id != job.Employer.Id)
                        return Forbid();
                    break;
            }

            var platformBalance = await _freelancingPlatform.BalanceManager.GetFreelancingPlatformBalanceAsync();
            var freelancerBalance =
                await _freelancingPlatform.BalanceManager.FindAsync(b => b.UserId == job.HiredFreelancer.Id);
            await _freelancingPlatform.BalanceManager.TransferMoneyAsync(senderBalance: platformBalance,
                recipientBalance: freelancerBalance, amount: job.Budget);
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
                return Forbid();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job?.HiredFreelancer is null || job.IsClosed)
                return UnprocessableEntity(jobId);

            job.IsPaymentDenied = true;
            await _freelancingPlatform.JobManager.UpdateAsync(job);

            var notification = new Notification
            {
                Receiver = job.HiredFreelancer,
                CreatedDate = DateTime.UtcNow,
                Source = Url.Action("JobInfo", "Job", new { jobId }),
                Title = $"{currentUser.UserName} wants to deny a job. Please follow the link and choose an action."
            };

            await _freelancingPlatform.NotificationManager.AddNotificationAsync(notification);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptDenying(int jobId)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || job.IsClosed || !job.IsPaymentDenied)
                return UnprocessableEntity(jobId);


            var platformBalance = await _freelancingPlatform.BalanceManager.GetFreelancingPlatformBalanceAsync();
            var employerBalance =
                await _freelancingPlatform.BalanceManager.FindAsync(b => b.UserId == job.Employer.Id);
            await _freelancingPlatform.BalanceManager.TransferMoneyAsync(senderBalance: platformBalance,
                recipientBalance: employerBalance, amount: job.Budget);
            job.IsClosed = true;
            await _freelancingPlatform.JobManager.UpdateAsync(job);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefuseDenying(int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer))
                return Forbid();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || job.IsClosed || !job.IsPaymentDenied)
                return UnprocessableEntity(jobId);

            await _freelancingPlatform.JobManager.UpdateAsync(job);
            var notification = new Notification
            {
                Receiver = await _moderatorManager.GetModeratorAsync(),
                CreatedDate = DateTime.UtcNow,
                Source = Url.Action("JobInfo", "Job", new { jobId }),
                Title = $"{job.Employer} wants to deny a job. Please follow the link and choose an action."
            };

            await _freelancingPlatform.NotificationManager.AddNotificationAsync(notification);
            return RedirectToAction("JobBoard", "Job");
        }
    }
}