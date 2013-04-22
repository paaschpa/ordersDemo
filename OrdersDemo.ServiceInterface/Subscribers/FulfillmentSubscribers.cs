using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface.Subscribers
{
    public class FulfillmentSubscribers : SubscribersBase
    {
        public FulfillmentSubscribers(Funq.Container container) : base(container)
        {
        }

        public void StartSubscriberThreads() //need to resolve dependencies...
        {
            //UpdateQueue when fulfillment it updated
            StartThread("", (channel, msg) =>
                    {
                        var updateRequest = msg.FromJson<UpdateFulfillment>();
                        var updateOrderInQueue = new UpdateOrderInQueue 
                        {
                            OrderId = updateRequest.OrderId,
                            Status = updateRequest.Status,
                            Fulfiller = updateRequest.Fulfiller
                        };
                        using (var service = Container.Resolve<OrderQueueService>())
                        {
                            service.Put(updateOrderInQueue);
                        }
                    });
        }
    }
}
