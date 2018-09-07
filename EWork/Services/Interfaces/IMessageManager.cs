using System.Linq;
using EWork.Data.Interfaces;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IMessageManager : IRepository<Message>
    {
        IQueryable<Message> GetChatHistory(string username1, string username2);
    }
}
