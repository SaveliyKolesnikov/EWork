using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Data.Extensions;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Repositories
{
    public class ProposalRepository : IRepository<Proposal>
    {
        private readonly IFreelancingPlatiformDbContext _db;
        private readonly UserManager<User> _userManager;

        public ProposalRepository(IFreelancingPlatiformDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task AddAsync(Proposal proposal)
        {
            if (!await _userManager.IsInRoleAsync(proposal.Sender, proposal.Sender.Role))
                throw new AuthenticationException($"User must be {proposal.Sender.Role} for doing this action.");

            try
            {
                _db.Freelancers.Attach(proposal.Sender);
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);
            }

            await _db.Proposals.AddAsync(proposal);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Proposal proposal)
        {
            if (proposal is null)
                return;

            _db.Proposals.Remove(proposal);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Proposal> items)
        {
            if (items is null)
                return;

            _db.Proposals.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Proposal proposal)
        {
            _db.Entry(proposal).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<Proposal> FindAsync(Predicate<Proposal> predicate) =>
            await _db.Proposals.FirstOrDefaultAsync(proposal => predicate(proposal));


        public IQueryable<Proposal> GetAll() => _db.Proposals.ExtractAll();
    }
}