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
    public class NotificationRepository : IRepository<Notification>
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public NotificationRepository(IFreelancingPlatiformDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Notification notification)
        {
            try
            {
                switch (notification.Receiver)
                {
                    case Employer employer:
                        _db.Employers.Attach(employer);
                        break;
                    case Freelancer freelancer:
                        _db.Freelancers.Attach(freelancer);
                        break;
                }
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);
            }

            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notification notification)
        {
            if (notification is null)
                return;

            _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Notification> items)
        {
            if (items is null)
                return;

            _db.Notifications.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _db.Entry(notification).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<Notification> FindAsync(Predicate<Notification> predicate) =>
            await _db.Notifications.ExtractAll().FirstOrDefaultAsync(notification => predicate(notification));

        public IQueryable<Notification> GetAll() => _db.Notifications.ExtractAll();
    }
}
