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
    public class FulfillmentSubscribers : SubscribersBase
    {
        public FulfillmentSubscribers(Funq.Container container) : base(container)
        {
        }

        public void StartSubscriberThreads() //need to resolve dependencies...
        {
            //Update an OrderInQueue when a fulfillment is updated 
            StartThread("FulfillmentUpdate", (channel, msg) =>
                    {
                        var fulfillmentRequest = msg.FromJson<Fulfillment>();
                        var updateOrderInQueue = new OrderInQueue 
                        {
                            OrderId = fulfillmentRequest.OrderId,
                            ItemName = fulfillmentRequest.ItemName,
                            Quantity = fulfillmentRequest.Quantity,
                            Status = fulfillmentRequest.Status,
                            Fulfiller = fulfillmentRequest.Fulfiller
                        };
                        using (var service = Container.Resolve<OrderQueueService>())
                        {
                            service.Put(updateOrderInQueue);
                        }

                        //Alert connections
                        var hub = GlobalHost.ConnectionManager.GetHubContext("GridHub");
                        if (hub != null)
                        {
                            hub.Clients.All.updateGrid(updateOrderInQueue);
                        }

                    });
        }

    }
}
