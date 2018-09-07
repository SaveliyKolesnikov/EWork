using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel(User currentUser, IQueryable<Message> messages, string pathToProfilePhotos, User receiver = null)
        {
            PathToProfilePhotos = pathToProfilePhotos;
            CurrentUser = currentUser;
            Messages = messages;
            Receiver = receiver;
        }

        public IQueryable<Message> Messages { get; }
        public User CurrentUser { get; }
        public User Receiver { get; }
        public string PathToProfilePhotos { get; }
    }
}
