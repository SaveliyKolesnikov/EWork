using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IModeratorManager
    {
        Task<Moderator> GetModeratorAsync();
    }
}