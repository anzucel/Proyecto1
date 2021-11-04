using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto1.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string Username, string Message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Username, Message);
        }
    }
}
