using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class ModeratorManager : IModeratorManager
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public ModeratorManager(IFreelancingPlatiformDbContext db) => _db = db;

        public async Task<Moderator> GetModeratorAsync()
        {
            var moderators = _db.Moderators.Include(m => m.Notifications);
            var minNotifications = await moderators.MinAsync(m => m.Notifications.Count);
            return await moderators.FirstOrDefaultAsync(m => m.Notifications.Count == minNotifications);
        }
    }
}
