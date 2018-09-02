using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel(Message message, User reciever = null)
        {
            Message = message;
            Reciever = reciever;
        }

        public Message Message { get; }
        public User Reciever { get; }
    }
}
