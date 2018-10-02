using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class ModeratorManager : IModeratorManager
    {
        private readonly IFreelancingPlatformDbContext _db;

        public ModeratorManager(IFreelancingPlatformDbContext db) => _db = db;

        public async Task<Moderator> GetAsync()
        {
            var moderators = _db.Moderators.Include(m => m.Notifications);
            var minNotifications = await moderators.MinAsync(m => m.Notifications.Count);
            return await moderators.FirstOrDefaultAsync(m => m.Notifications.Count == minNotifications);
        }
    }
}
