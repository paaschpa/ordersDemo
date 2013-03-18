using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace OrdersDemo.SignalR
{
    public class GridHub : Hub
    {
        public void Refresh(string message)
        {
            Clients.All.refreshGrid(message);
        }
    }
}