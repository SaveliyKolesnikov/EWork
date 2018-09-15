using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class NotificationViewModel
    {
        public NotificationViewModel(User currentUser, IQueryable<Notification> notifications)
        {
            CurrentUser = currentUser;
            Notifications = notifications;
        }

        public User CurrentUser { get; }
        public IQueryable<Notification> Notifications { get; }
    }
}
