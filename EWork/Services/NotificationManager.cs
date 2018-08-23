using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EWork.Services
{
    public class NotificationManager : INotificationManager
    {
        private readonly UserManager<User> _userManager;

        public NotificationManager(UserManager<User> userManager) => _userManager = userManager;

        public async Task AddNotificationAsync(User user, Notification notification)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (notification is null)
                throw new ArgumentNullException(nameof(notification));

            if (user.Notifications is null)
                user.Notifications = new List<Notification>();

            user.Notifications.Add(notification);
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteNotificationAsync(User user, Notification notification)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (notification is null)
                throw new ArgumentNullException(nameof(notification));

            if (user.Notifications is null)
            {
                user.Notifications = new List<Notification>();
                return;
            }

            if (user.Notifications.All(n => n.Id != notification.Id))
                return;

            user.Notifications.Remove(notification);
            await _userManager.UpdateAsync(user);
        }
    }
}
