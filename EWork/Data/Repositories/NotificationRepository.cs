﻿using System;
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
        private readonly IFreelancingPlatiformDbContext _db;

        public NotificationRepository(IFreelancingPlatiformDbContext db) => _db = db;

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
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            if (await GetAll().AnyAsync(n => n.Receiver.Id == notification.Receiver.Id
                                             && n.Source == notification.Source))
            { 
                return;
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
            await GetAll().FirstOrDefaultAsync(notification => predicate(notification));

        public IQueryable<Notification> GetAll() => _db.Notifications.ExtractAll();
    }
}
