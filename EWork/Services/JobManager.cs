using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class JobManager : IJobManager
    {
        private readonly IRepository<Job> _repository;

        public JobManager(IRepository<Job> repository) => _repository = repository;

        public Task AddAsync(Job item) => _repository.AddAsync(item);

        public Task DeleteAsync(Job item)
        {
            if (!item.IsClosed && !(item.HiredFreelancer is null))
                throw new ArgumentException("Job isn't closed.", nameof(item));

            return _repository.DeleteAsync(item);
        }

        public async Task DeleteRangeAsync(IEnumerable<Job> items) => await _repository.DeleteRangeAsync(items);

        public Task<Job> FindAsync(Predicate<Job> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Job> GetAll() => _repository.GetAll();

        public Task UpdateAsync(Job item) => _repository.UpdateAsync(item);
    }
}