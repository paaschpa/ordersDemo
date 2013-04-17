using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.CacheAccess;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace OrdersDemo.ServiceInterface.Subscribers
{
    public class OrderSubscribers : SubscribersBase
    {
        public OrderSubscribers(Funq.Container container) : base(container)
        {
        }

        public void StartSubscriberThreads() //need to resolve dependencies...
        {
            //Create a fulfillment when an Order is posted
            StartThread("NewOrder", (channel, msg) =>
                    {
                        var createOrderRequest = msg.FromJson<Order>();
                        var createFulfillment = new CreateFulfillment
                        {
                            OrderId = createOrderRequest.Id,
                            ItemName = createOrderRequest.ItemName,
                            Quantity = createOrderRequest.Quantity
                        };
                        using (var service = Container.Resolve<FulfillmentService>())
                        {
                            service.Post(createFulfillment);
                        }

                        //Alert connections
                        var hub = GlobalHost.ConnectionManager.GetHubContext("GridHub");
                        if (hub != null)
                        {
                            hub.Clients.All.refreshGrid("newOrder");
                        }
                    });

            //Create an Order in the Queue when an Order is posted
            StartThread("NewOrder", (channel, msg) =>
                    {
                        var createOrderRequest = msg.FromJson<Order>();
                        var createOrderInQueue = new OrderInQueue
                        {
                            OrderId = createOrderRequest.Id,
                            CustomerName = createOrderRequest.CustomerFirstName,
                            ItemName = createOrderRequest.ItemName,
                            Status = "New"
                        };
                        using (var service = Container.Resolve<OrderQueueService>())
                        {
                            service.Post(createOrderInQueue);
                        }

                        //Alert connections
                        var hub = GlobalHost.ConnectionManager.GetHubContext("GridHub");
                        if (hub != null)
                        {
                            hub.Clients.All.refreshGrid("newOrder");
                        }
                    });
        }
    }
}
