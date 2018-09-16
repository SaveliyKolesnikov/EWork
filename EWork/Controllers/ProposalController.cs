using System;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    public class ProposalController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public ProposalController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "freelancer, moderator, administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProposal(int proposalId)
        {
            var deletedProposal = await _freelancingPlatform.ProposalManager.FindAsync(proposal => proposal.Id == proposalId);
            if (deletedProposal is null)
                return UnprocessableEntity(proposalId);

            var currentUser = await _userManager.GetUserAsync(User);
            var isCurrentUserFromAdministration = currentUser is Administrator || currentUser is Moderator;
            if (!isCurrentUserFromAdministration && deletedProposal.Sender.Id != currentUser.Id)
                return Forbid();

            await _freelancingPlatform.ProposalManager.DeleteAsync(deletedProposal);

            if (isCurrentUserFromAdministration)
            {
                var notification = new Notification
                {
                    Receiver = deletedProposal.Sender,
                    Title =
                        $"{FirstCharToUpper(currentUser.Role)} {currentUser.UserName} deleted your proposal on the job \"{deletedProposal.Job.Title}\".",
                    Source = Url.Action("JobInfo", "Job", new { jobid = deletedProposal.Job.Id }),
                    CreatedDate = DateTime.UtcNow
                };
                await _freelancingPlatform.NotificationManager.AddNotificationAsync(notification);
            }

            return RedirectToAction("JobInfo", "Job", new {jobid = deletedProposal.Job.Id});

            string FirstCharToUpper(string input)
            {
                switch (input)
                {
                    case null: throw new ArgumentNullException(nameof(input));
                    case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                    default: return input.First().ToString().ToUpper() + input.Substring(1);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal(Proposal proposal, int jobId)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return Forbid();

            var wantedJob = await _freelancingPlatform.JobManager.FindAsync(job => job.Id == jobId);
            if (wantedJob is null || wantedJob.IsClosed ||
                !(wantedJob.HiredFreelancer is null) ||
                wantedJob.Proposals.Any(p => p.Sender.Id == currentUser.Id))
                return Forbid();

            proposal.Sender = currentUser;
            proposal.SendDate = DateTime.Now;
            proposal.Job = wantedJob;

            await _freelancingPlatform.ProposalManager.AddAsync(proposal);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProposal(Proposal proposal, bool isDeleting)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            if (isDeleting)
                return await DeleteProposal(proposal.Id);

            var oldProposal = await _freelancingPlatform.ProposalManager.FindAsync(p => p.Id == proposal.Id);
            if (oldProposal is null || oldProposal.Job.IsClosed || oldProposal.Job.IsPaymentDenied)
                return Forbid();

            oldProposal.SendDate = DateTime.Now;
            oldProposal.Text = proposal.Text;

            await _freelancingPlatform.ProposalManager.UpdateAsync(oldProposal);
            return RedirectToAction("JobBoard", "Job");
        }


        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptProposal(int jobId, int proposalId)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Forbid();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || !(job.HiredFreelancer is null) || job.Employer.Id != currentUser.Id ||
                job.IsClosed || job.IsPaymentDenied)
                return Forbid();

            var proposal = job.Proposals.FirstOrDefault(p => p.Id == proposalId);
            if (proposal?.Sender is null)
                return UnprocessableEntity(proposalId);

            job.HiredFreelancer = proposal.Sender;
            var deletedProposals = job.Proposals.Except(new[] { proposal });
            job.Proposals.Clear();
            job.Proposals.Add(proposal);

            await _freelancingPlatform.ProposalManager.DeleteRangeAsync(deletedProposals);
            await _freelancingPlatform.JobManager.UpdateAsync(job);

            return RedirectToAction("JobInfo", "Job", new { jobId });
        }
    }
}