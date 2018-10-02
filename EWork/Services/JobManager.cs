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
        private readonly IJobRecommender _jobRecommender;

        public JobManager(IRepository<Job> repository, IJobRecommender jobRecommender)
        {
            _repository = repository;
            _jobRecommender = jobRecommender;
        }

        public async Task AddAsync(Job item)
        {
            await _repository.AddAsync(item);
            await _jobRecommender.RecommendAsync(await FindAsync(j => j.CreationDate == item.CreationDate && j.Budget == item.Budget && j.Title == item.Title));
        }

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