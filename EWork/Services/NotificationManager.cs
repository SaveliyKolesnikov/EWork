using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class NotificationManager : INotificationManager
    {
        private readonly IRepository<Notification> _repository;
        private readonly UserManager<User> _userManager;

        public NotificationManager(IRepository<Notification> repository, UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task AddNotificationAsync(Notification notification, User user = null)
        {
            if (!(user?.Notifications is null) &&
                user.Notifications.Any(n => n.Receiver.Id == notification.Receiver.Id &&
                                            n.Source == notification.Source))
            { 
                return;
            }

            await _repository.AddAsync(notification);
        }

        public async Task DeleteNotificationAsync(User user, Notification notification)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (notification is null)
                throw new ArgumentNullException(nameof(notification));

            if (user.Notifications is null)
            {
                await _repository.DeleteAsync(notification);
            }
            else
            {
                if (user.Notifications.All(n => n.Id != notification.Id))
                    return;

                user.Notifications.Remove(notification);
                await _userManager.UpdateAsync(user);
            }
        }

        public Task AddAsync(Notification item) => _repository.AddAsync(item);

        public Task DeleteAsync(Notification item) => _repository.DeleteAsync(item);

        public async Task DeleteRangeAsync(IEnumerable<Notification> items) => await _repository.DeleteRangeAsync(items);

        public Task<Notification> FindAsync(Predicate<Notification> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Notification> GetAll() => _repository.GetAll();

        public Task UpdateAsync(Notification item) => _repository.UpdateAsync(item);
    }
}
