using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface INotificationManager
    {
        Task AddNotificationAsync(User user, Notification notification);

        Task DeleteNotificationAsync(User user, Notification notification);
    }
}
