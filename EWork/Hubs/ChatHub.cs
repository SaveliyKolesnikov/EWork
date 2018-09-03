using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Models.Json;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task SendMessage(JsonMessage message)
        {

            var sender = await _userManager.GetUserAsync(Context.User);
            if (message.Sender.UserName != sender.UserName)
                throw new AuthenticationException();
            var receiver = await _userManager.FindByNameAsync(message.Receiver.UserName) ??
                           throw new ArgumentException($"User with user name {message.Receiver.UserName} doesn't exist");

            if (string.IsNullOrWhiteSpace(message.Text))
                throw new ArgumentNullException(nameof(message.Text));

            if (message.Text.Length > 4096)
                throw new ArgumentException("Message text length must be less then 4096");

            await _messageManager.AddAsync(new Message
            {
                Receiver = receiver, Sender = sender, SendDate = message.SendDate, Text = message.Text
            });

            var receiverId = Connections.GetValueOrDefault(message.Receiver.UserName);
            if (!(receiverId is null))
                await Clients.Client(receiverId).SendAsync("receiveMessage", message);
            else
            {
                // TODO: Notification
            }
            await Clients.Caller.SendAsync("receiveMessage", message);
        }
    }
}
