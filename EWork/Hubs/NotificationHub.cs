using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EWork.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private static readonly ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
        public static IReadOnlyDictionary<string, string> UsersConnections => Connections;

        public NotificationHub(UserManager<User> userManager)
        {
            _userManager = userManager;
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

        public async Task SendNotificationAsync(Notification notification)
        {
            if (Connections.ContainsKey(notification.Receiver.UserName) && 
                Connections.TryGetValue(notification.Receiver.UserName, out var id))
                await Clients.Client(id).SendAsync("NewNotification");
        }
    }
}
