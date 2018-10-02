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
    public class BalanceRepository : IBalanceRepository
    {
        private readonly IFreelancingPlatformDbContext _db;

        public BalanceRepository(IFreelancingPlatformDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Balance balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            await _db.Balances.AddAsync(balance);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Balance balances)
        {
            if (balances is null)
                return;

            _db.Balances.Remove(balances);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Balance> items)
        {
            if (items is null)
                return;

            _db.Balances.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Balance balance)
        {
            _db.Entry(balance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Balance> items)
        {
            foreach (var balance in items)
            {
                _db.Entry(balance).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Balance> FindAsync(Predicate<Balance> predicate) =>
            await GetAll().FirstOrDefaultAsync(balance => predicate(balance));

        public IQueryable<Balance> GetAll() => _db.Balances.ExtractAll();
    }
}