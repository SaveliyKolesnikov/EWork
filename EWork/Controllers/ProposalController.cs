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
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProposal(int proposalId)
        {
            var deletedProposal = await _freelancingPlatform.ProposalManager.FindAsync(proposal => proposal.Id == proposalId);
            if (deletedProposal is null)
                return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);
            if (deletedProposal.Sender.Id != currentUser.Id)
                return BadRequest();

            await _freelancingPlatform.ProposalManager.DeleteAsync(deletedProposal);
            return RedirectToAction("JobBoard", "Job");
        }

        [HttpPost]
        [Authorize(Roles = "freelancer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal(Proposal proposal, int jobId)
        {
            if (!ModelState.IsValid)
                return BadRequest(); //TODO: Сделать возвращение JSON с сообщением об ошибке

            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return BadRequest();

            var wantedJob = await _freelancingPlatform.JobManager.FindAsync(job => job.Id == jobId);
            if (wantedJob is null || wantedJob.IsClosed ||
                !(wantedJob.HiredFreelancer is null) ||
                wantedJob.Proposals.Any(p => p.Sender.Id == currentUser.Id))
                return BadRequest();

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
                return BadRequest(); //TODO: Сделать возвращение JSON с сообщением об ошибке

            if (isDeleting)
                return await DeleteProposal(proposal.Id);

            var oldProposal = await _freelancingPlatform.ProposalManager.FindAsync(p => p.Id == proposal.Id);
            if (oldProposal is null || oldProposal.Job.IsClosed || oldProposal.Job.IsPaymentDenied)
                return BadRequest();

            oldProposal.SendDate = DateTime.Now;
            oldProposal.Text = proposal.Text;

            await _freelancingPlatform.ProposalManager.UpdateAsync(oldProposal);
            return RedirectToAction("JobBoard", "Job");
        }

        [Authorize(Roles = "freelancer")]
        public async Task<IActionResult> AllFreelancerProposals()
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return BadRequest();

            var jobs = _freelancingPlatform.JobManager.GetAll()
                .Where(j => j.Proposals.Any(p => !p.Job.IsClosed && p.Sender.Id == currentUser.Id));

            ViewData["Title"] = "Proposals";
            ViewBag.Heading = "Jobs with Your Proposal";
            return View("~/Views/Job/JobBoard.cshtml", jobs);
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptProposal(int jobId, int proposalId)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return BadRequest();

            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == jobId);
            if (job is null || !(job.HiredFreelancer is null) || job.Employer.Id != currentUser.Id ||
                job.IsClosed || job.IsPaymentDenied)
                return BadRequest();

            var proposal = job.Proposals.FirstOrDefault(p => p.Id == proposalId);
            if (proposal?.Sender is null)
                return BadRequest();

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