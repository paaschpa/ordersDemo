using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack;

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
            StartThread("NewOrder", (channel, msg) => TryWrapper(() => 
                    {
                        Fulfillment newFulfillment;
                        
                        var createOrderRequest = msg.FromJson<Order>();
                        var createFulfillment = new CreateFulfillment
                        {
                            OrderId = createOrderRequest.Id,
                            ItemId = createOrderRequest.ItemId,
                            ItemName = createOrderRequest.ItemName,
                            Quantity = createOrderRequest.Quantity
                        };
                        using (var service = Container.Resolve<FulfillmentService>())
                        {
                            newFulfillment = service.Post(createFulfillment);
                        }

                        //Alert connections
                        var hub = GlobalHost.ConnectionManager.GetHubContext("FulfillmentGridHub");
                        if (hub != null)
                        {
                            hub.Clients.All.addToGrid(newFulfillment);
                        }
                    }));

            //Create an Order in the Queue when an Order is posted
            StartThread("NewOrder", (channel, msg) => TryWrapper(() => 
                    {
                        var createOrderRequest = msg.FromJson<Order>();
                        var createOrderInQueue = new OrderInQueue
                        {
                            OrderId = createOrderRequest.Id,
                            CustomerName = createOrderRequest.CustomerFirstName,
                            ItemId = createOrderRequest.ItemId,
                            ItemName = createOrderRequest.ItemName,
                            Quantity = createOrderRequest.Quantity,
                            Status = "New"
                        };
                        using (var service = Container.Resolve<OrderQueueService>())
                        {
                            service.Post(createOrderInQueue);
                        }

                        //Alert connections
                        var hub = GlobalHost.ConnectionManager.GetHubContext("OrdersQueueGridHub");
                        if (hub != null)
                        {
                            hub.Clients.All.addToGrid(createOrderInQueue);
                        }
                    }));
        }
    }
}
