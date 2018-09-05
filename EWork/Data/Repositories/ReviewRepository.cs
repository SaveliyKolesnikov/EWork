using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Extensions;
using EWork.Data.Interfaces;
using EWork.Models;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Repositories
{
    public class ReviewRepository : IRepository<Review>
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public ReviewRepository(IFreelancingPlatiformDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Review review)
        {
            if (review is null)
                throw new ArgumentNullException(nameof(review));

            try
            {
                AttachUserToDbContext(review.User);
                AttachUserToDbContext(review.Sender);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            await _db.Reviews.AddAsync(review);
            await _db.SaveChangesAsync();

            void AttachUserToDbContext(User user)
            {
                switch (user)
                {
                    case null:
                        throw new ArgumentNullException(nameof(user));
                    case Employer employer:
                        _db.Employers.Attach(employer);
                        break;
                    case Freelancer freelancer:
                        _db.Freelancers.Attach(freelancer);
                        break;
                    case Moderator moderator:
                        _db.Moderators.Attach(moderator);
                        break;
                }
            }
        }

        public async Task DeleteAsync(Review review)
        {
            if (review is null)
                throw new ArgumentNullException(nameof(review));

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Review> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            _db.Reviews.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            if (review is null)
                throw new ArgumentNullException(nameof(review));

            try
            {
                _db.Entry(review).State = EntityState.Modified;
            }
            catch (InvalidOperationException e)
            {
                if (!e.Message.Contains("'Review' cannot be tracked"))
                    throw;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Review> FindAsync(Predicate<Review> predicate) =>
            await GetAll().FirstOrDefaultAsync(review => predicate(review));

        public IQueryable<Review> GetAll() => _db.Reviews.ExtractAll();
    }
}
