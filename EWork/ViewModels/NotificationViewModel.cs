using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class NotificationViewModel
    {
        public NotificationViewModel(User currentUser, IQueryable<Notification> notifications)
            => (CurrentUser, Notifications) = (currentUser, notifications);

        public User CurrentUser { get; }
        public IQueryable<Notification> Notifications { get; }
    }
}
