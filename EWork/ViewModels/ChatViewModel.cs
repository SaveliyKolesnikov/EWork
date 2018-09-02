using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel(IQueryable<Message> messages, string pathToProfilePhotos, User receiver = null)
        {
            PathToProfilePhotos = pathToProfilePhotos;
            Messages = messages;
            Receiver = receiver;
        }

        public IQueryable<Message> Messages { get; }
        public User Receiver { get; }
        public string PathToProfilePhotos { get; }
    }
}
