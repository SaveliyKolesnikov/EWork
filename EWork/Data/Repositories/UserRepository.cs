using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Data.Extensions;
using EWork.Data.Interfaces;
using EWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IFreelancingPlatiformDbContext _db;
        private readonly UserManager<User> _userManager;

        public UserRepository(IFreelancingPlatiformDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Obsolete("AddAsync user async is deprecated, please use UserManager method instead.")]
        public void Add(User user)
        {
            throw new NotImplementedException("AddAsync user async is deprecated, please use UserManager method instead.");
        }

        public Task DeleteAsync(User user) => _userManager.DeleteAsync(user);

        public async Task DeleteRangeAsync(IEnumerable<User> items)
        {
            foreach (var user in items)
            {
                await DeleteAsync(user);
            }
        }

        public Task UpdateAsync(User user) => _userManager.UpdateAsync(user);

        public async Task<User> FindAsync(Predicate<User> predicate) =>
            await GetAll().FirstOrDefaultAsync(user => predicate(user));

        public IQueryable<User> GetAll() => _db.Employers.ExtractAll().Union(_db.Freelancers.ExtractAll());
    }
}