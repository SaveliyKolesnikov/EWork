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
    public class NotificationRepository : IRepository<Notification>
    {
        private readonly IFreelancingPlatformDbContext _db;

        public NotificationRepository(IFreelancingPlatformDbContext db) => _db = db;

        public async Task AddAsync(Notification notification)
        {
            if (notification is null)
                throw new ArgumentNullException(nameof(notification));

            try
            {
                switch (notification.Receiver)
                {
                    case null:
                        throw new ArgumentNullException(nameof(notification.Receiver));
                    case Employer employer:
                        _db.Employers.Attach(employer);
                        break;
                    case Freelancer freelancer:
                        _db.Freelancers.Attach(freelancer);
                        break;
                    case Moderator moderator:
                        _db.Moderators.Attach(moderator);
                        break;
                    case Administrator administrator:
                        _db.Administrators.Attach(administrator);
                        break;
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            //var similarNotification = await _db.Notifications.Include(n => n.Receiver)
            //    .FirstOrDefaultAsync(n => n.Receiver.Id == notification.Receiver.Id && n.Source == notification.Source);
            //if (!(similarNotification is null))
            //{ 
            //    similarNotification.CreatedDate = DateTime.UtcNow;
            //    await UpdateAsync(similarNotification);
            //    return;
            //}

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

        public Task<Notification> FindAsync(Predicate<Notification> predicate) =>
            GetAll().FirstOrDefaultAsync(notification => predicate(notification));

        public IQueryable<Notification> GetAll() => _db.Notifications.ExtractAll();
    }
}
