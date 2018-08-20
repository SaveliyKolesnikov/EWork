using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Data;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class EWork : IFreelancingPlatform
    {
        private readonly IFreelancingPlatiformDbContext _db;
        private readonly UserManager<User> _userManager;

        public EWork(IFreelancingPlatiformDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task AddJobAsync(Job job)
        {
            if (!await _userManager.IsInRoleAsync(job.Employer, job.Employer.Role))
                throw new AuthenticationException($"User must be {job.Employer.Role} for doing this action.");

            await _db.Jobs.AddAsync(job);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(Job job)
        {
            _db.Jobs.Remove(job);
            await _db.SaveChangesAsync();
        }

        public async Task<Job> FindJobAsync(Predicate<Job> predicate) => 
            await _db.Jobs.FirstOrDefaultAsync(job => predicate(job));


        public IQueryable<Job> Jobs => _db.Jobs.AsQueryable();
    

        public async Task AddProposalAsync(Proposal proposal)
        {
            if (!await _userManager.IsInRoleAsync(proposal.Sender, proposal.Sender.Role))
                throw new AuthenticationException($"User must be {proposal.Sender.Role} for doing this action.");

            await _db.Proposals.AddAsync(proposal);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProposalAsync(Proposal proposal)
        {
            _db.Proposals.Remove(proposal);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateProposal(Proposal proposal)
        {
            _db.Entry(proposal).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<Proposal> FindProposalAsync(Predicate<Proposal> predicate) =>
            await _db.Proposals.FirstOrDefaultAsync(proposal => predicate(proposal));

        public IQueryable<Proposal> Proposals => _db.Proposals.AsQueryable();
    }
}
