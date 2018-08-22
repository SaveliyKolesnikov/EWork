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
    [Authorize(Roles = "freelancer")]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal(Proposal proposal, int jobId)
        {
            if (!(await _userManager.GetUserAsync(User) is Freelancer currentUser))
                return BadRequest();

            var wantedJob = await _freelancingPlatform.JobManager.FindAsync(job => job.Id == jobId);
            if (wantedJob is null ||
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProposal(Proposal proposal, bool isDeleting)
        {
            if (isDeleting)
                return await DeleteProposal(proposal.Id);

            var oldProposal = await _freelancingPlatform.ProposalManager.FindAsync(p => p.Id == proposal.Id);
            if (oldProposal is null)
                return BadRequest();

            oldProposal.SendDate = DateTime.Now;
            oldProposal.Text = proposal.Text;

            await _freelancingPlatform.ProposalManager.UpdateAsync(oldProposal);
            return RedirectToAction("JobBoard", "Job");
        }
    }
}