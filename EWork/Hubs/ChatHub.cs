using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EWork.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageManager _messageManager;
        private static readonly ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();

        public ChatHub(UserManager<User> userManager, IMessageManager messageManager)
        {
            _userManager = userManager;
            _messageManager = messageManager;
        }

        public override Task OnConnectedAsync()
        {
            var name = _userManager.GetUserName(Context.User);

            Connections.TryAdd(name, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = _userManager.GetUserName(Context.User);

            Connections.TryRemove(name, out var id);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderUserName, string receiverUserName, string text)
        {

            var sender = await _userManager.GetUserAsync(Context.User);
            if (senderUserName != sender.UserName)
                throw new AuthenticationException();
            var receiver = await _userManager.FindByNameAsync(receiverUserName) ??
                           throw new ArgumentException($"User with user name {receiverUserName} doesn't exist");

            var message = new Message
            {
                SendDate = DateTime.UtcNow,
                Sender = sender,
                Receiver = receiver,
                Text = text
            };

            if (string.IsNullOrWhiteSpace(message.Text))
                throw new ArgumentNullException(nameof(message.Text));
            if (message.Text.Length > 4096)
                throw new ArgumentException("Message text length must be less then 4096");

            await _messageManager.AddAsync(message);


            var messageForUsers = new
            {
                senderUserName,
                receiverUserName,
                text = message.Text,
                sendDate = message.SendDate
            };

            var receiverId = Connections.GetValueOrDefault(receiverUserName);
            if (!(receiverId is null)) 
                await Clients.Client(receiverId).SendAsync("receiveMessage", messageForUsers);
            else
            {
                // TODO: Notification
            }
            await Clients.Caller.SendAsync("receiveMessage", messageForUsers);
        }
    }
}
