using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class NotificationViewModel
    {
        public NotificationViewModel(User currentUser, IEnumerable<Notification> notifications, int takeAmount = 5)
        {
            CurrentUser = currentUser;
            Notifications = notifications;
            TakeAmount = takeAmount;
        }

        public User CurrentUser { get; }
        public IEnumerable<Notification> Notifications { get; }
        public int TakeAmount { get; }
    }
}
