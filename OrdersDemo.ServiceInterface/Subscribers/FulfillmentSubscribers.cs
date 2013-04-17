using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.CacheAccess;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace OrdersDemo.ServiceInterface.Subscribers
{
    public class FulfillmentSubscribers
    {
        private Funq.Container _container;
        public FulfillmentSubscribers(Funq.Container container)
        {
            _container = container;
        }

        public void StartSubscriberThreads() //need to resolve dependencies...
        {
            //Update an OrderInQueue when a fulfillment is updated 
            StartThread("FulfillmentUpdate", (channel, msg) =>
                    {
                        var fulfillmentRequest = msg.FromJson<Fulfillment>();
                        var createFulfillment = new OrderInQueue 
                        {
                            ItemName = fulfillmentRequest.ItemName,
                            Quantity = fulfillmentRequest.Quantity,
                            Status = fulfillmentRequest.Status
                        };
                        using (var service = _container.Resolve<OrderQueueService>())
                        {
                            service.Put(createFulfillment);
                        }
                    });
        }

        private void StartThread(string subscribeToChannelName, Action<string, string> onMessage)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var redisConsumer = _container.Resolve<IRedisClientsManager>().GetClient())
                using (var subscription = redisConsumer.CreateSubscription())
                {
                    subscription.OnSubscribe = channel =>
                    {
                        Console.WriteLine("listening for orders on channel {0}", channel);
                    };
                    subscription.OnUnSubscribe = channel =>
                    {
                        Console.WriteLine("UnSubscribed from '{0}'", channel);
                    };

                    subscription.OnMessage = onMessage;

                    subscription.SubscribeToChannels(subscribeToChannelName); //blocking
                }
            });
        }
    }
}
