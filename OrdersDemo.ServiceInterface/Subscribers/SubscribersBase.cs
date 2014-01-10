using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Funq;
using ServiceStack.Redis;

namespace OrdersDemo.ServiceInterface.Subscribers
{
    public abstract class SubscribersBase
    {
        protected readonly Funq.Container Container;

        protected SubscribersBase(Funq.Container container)
        {
            this.Container = container;
        }

        protected virtual void StartThread(string subscribeToChannelName, Action<string, string> onMessage)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var redisConsumer = Container.Resolve<IRedisClientsManager>().GetClient())
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

        protected virtual void TryWrapper(Action act)
        {
            try
            {
                act();
            }
            catch (Exception)
            {
                //TODO:This is probably a bad way to implement this try but I don't have to write all the try catches and I just want to check 
                //if exceptions are killing me threads 
            }
        }

    }
}
