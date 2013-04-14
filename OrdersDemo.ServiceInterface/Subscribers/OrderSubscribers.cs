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
    public class OrderSubscribers
    {
        private Funq.Container _container;
        public OrderSubscribers(Funq.Container container)
        {
            _container = container;
        }

        public void StartSubscriberThreads() //need to resolve dependencies...
        {
            //Create a fulfillment when an Order is posted
            StartThread("NewOrder", (channel, msg) =>
                    {
                        var createOrderRequest = msg.FromJson<CreateOrder>();
                        var createFulfillment = new CreateFulfillment
                        {
                            ItemName = createOrderRequest.ItemName,
                            Quantity = createOrderRequest.Quantity
                        };
                        using (var service = _container.Resolve<FulfillmentService>())
                        {
                            service.Post(createFulfillment);
                        }
                    });

            //Create an Order in the Queue when an Order is posted
            StartThread("NewOrder", (channel, msg) =>
                    {
                        var createOrderRequest = msg.FromJson<CreateOrder>();
                        var createOrderInQueue = new OrderInQueue
                        {
                            CustomerName = createOrderRequest.CustomerName,
                            Status = "New"
                        };
                        using (var service = _container.Resolve<OrderQueueService>())
                        {
                            service.Post(createOrderInQueue);
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
