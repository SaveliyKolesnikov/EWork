using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface INotificationManager : IRepository<Notification>
    {
        Task AddNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(User user, Notification notification);
    }
}
