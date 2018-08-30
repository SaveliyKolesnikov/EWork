using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class ReviewManager : IReviewManager
    {
        private readonly IRepository<Review> _repository;

        public ReviewManager(IRepository<Review> repository) => _repository = repository;

        public Task AddAsync(Review item) => _repository.AddAsync(item);

        public Task DeleteAsync(Review item) => _repository.DeleteAsync(item);

        public async Task DeleteRangeAsync(IEnumerable<Review> items) => await _repository.DeleteRangeAsync(items);

        public Task<Review> FindAsync(Predicate<Review> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Review> GetAll() => _repository.GetAll();

        public Task UpdateAsync(Review item) => _repository.UpdateAsync(item);
    }
}
