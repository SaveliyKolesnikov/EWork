using System;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IFreelancingPlatform
    {
        #region Job
        Task AddJobAsync(Job job);
        Task DeleteJobAsync(Job job);
        Task<Job> FindJobAsync(Predicate<Job> predicate);
        IQueryable<Job> Jobs { get; }
        #endregion

        #region Proposal
        Task AddProposalAsync(Proposal proposal);
        Task DeleteProposalAsync(Proposal proposal);
        Task UpdateProposal(Proposal proposal);
        Task<Proposal> FindProposalAsync(Predicate<Proposal> predicate);
        IQueryable<Proposal> Proposals { get; }
        #endregion
    }
}