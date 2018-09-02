using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatAuth.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAuth.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message) =>
            await Clients.All.SendAsync("receiveMessage", message);
    }
}
