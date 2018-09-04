using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Models.Json;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace EWork.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageManager _messageManager;
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<PhotoConfig> _photoOptions;
        private static readonly ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();

        public ChatHub(UserManager<User> userManager, IMessageManager messageManager, IHostingEnvironment environment, IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _messageManager = messageManager;
            _environment = environment;
            _photoOptions = photoOptions;
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
                Receiver = receiver,
                Sender = sender,
                SendDate = message.SendDate,
                Text = message.Text
            });

            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);
            message.Sender.PhotoUrl = Path.Combine(pathToProfilePhotos, sender.ProfilePhotoName);
            message.Receiver.PhotoUrl = Path.Combine(pathToProfilePhotos, receiver.ProfilePhotoName);

            var receiverId = Connections.GetValueOrDefault(message.Receiver.UserName);
            if (!(receiverId is null))
            {
                await Clients.Client(receiverId).SendAsync("receiveMessage", message);
            }
            else
            {
                // TODO: Notification
            }
            await Clients.Caller.SendAsync("receiveMessage", message);
        }
    }
}
