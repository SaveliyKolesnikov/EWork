using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel(User currentUser, IQueryable<Message> messages, string pathToProfilePhotos, User receiver = null) 
            => (CurrentUser, Messages, PathToProfilePhotos, Receiver) = (currentUser, messages, pathToProfilePhotos, receiver);

        public IQueryable<Message> Messages { get; }
        public User CurrentUser { get; }
        public User Receiver { get; }
        public string PathToProfilePhotos { get; }
    }
}
