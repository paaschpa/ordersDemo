using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace OrdersDemo.SignalR
{
    public class FulfillmentGridHub : Hub
    {
        public void Refresh(string message)
        {
            Clients.All.refreshGrid(message);
        }
    }

    public class OrdersQueueGridHub : Hub
    {
        public void Refresh(string message)
        {
            Clients.All.refreshGrid(message);
        }
    }
}