using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IFreelancingPlatformDbContext _db;
        private readonly UserManager<User> _userManager;

        public UserRepository(IFreelancingPlatformDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Obsolete("AddAsync user async is deprecated. Please, use UserManager method instead.")]
        public Task AddAsync(User user)
        {
            throw new NotImplementedException("AddAsync user async is deprecated. Please, use UserManager method instead.");
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

        public IQueryable<User> GetAll() => _db.Users.ExtractAll();
    }
}