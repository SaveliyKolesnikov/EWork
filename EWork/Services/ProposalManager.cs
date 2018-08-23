using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class ProposalManager : IProposalManager
    {
        private readonly IRepository<Proposal> _repository;

        public ProposalManager(IRepository<Proposal> repository) => _repository = repository;

        public Task AddAsync(Proposal item) => _repository.AddAsync(item);

        public Task DeleteAsync(Proposal item) => _repository.DeleteAsync(item);
        public async Task DeleteRangeAsync(IEnumerable<Proposal> items) => await _repository.DeleteRangeAsync(items);

        public Task<Proposal> FindAsync(Predicate<Proposal> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Proposal> GetAll() => _repository.GetAll();

        public Task UpdateAsync(Proposal item) => _repository.UpdateAsync(item);
    }
}