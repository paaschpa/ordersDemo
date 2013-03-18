using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using OrdersDemo.Services;
using ServiceStack.Configuration;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

[assembly: WebActivator.PreApplicationStartMethod(typeof(OrdersDemo.App_Start.AppHost), "Start")]

namespace OrdersDemo.App_Start
{
	public class AppHost
		: AppHostBase
	{		
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("Orders Demo", typeof(OrdersService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            var dataFilePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\data.db";
		    Container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(dataFilePath, SqliteDialect.Provider));

            container.Register<IRedisClientsManager>(new BasicRedisClientManager("localhost:6379"));
            container.Register<ICacheClient>(c =>(ICacheClient)c.Resolve<IRedisClientsManager>().GetCacheClient());

		    using (var con = AppHostBase.Resolve<IDbConnectionFactory>().OpenDbConnection())
		    {
                con.CreateTable<Order>();
                con.CreateTable<Fulfillment>();
		    }

		    //Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));


            //Listen for New Orders
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var redisConsumer = AppHost.Resolve<IRedisClientsManager>().GetClient())
                using (var fulfillmentOrderSubscription = redisConsumer.CreateSubscription())
                {
                    var messagesReceived = 0;
                    fulfillmentOrderSubscription.OnSubscribe = channel =>
                    {
                        Console.WriteLine("Fulfillment listening for orders on channel {0}", channel);
                    };
                    fulfillmentOrderSubscription.OnUnSubscribe = channel =>
                    {
                        Console.WriteLine("Fulfillment UnSubscribed from '{0}'", channel);
                    };

                    fulfillmentOrderSubscription.OnMessage = (channel, msg) =>
                        {
                            var createOrderRequest = msg.FromJson<CreateOrder>(); 
                            var createFulfillment = new CreateFulfillment
                                {
                                    ItemName = createOrderRequest.ItemName,
                                    Quantity = createOrderRequest.Quantity
                                };
                            using (var service = AppHostBase.Resolve<FulfillmentService>())
                            {
                                service.Post(createFulfillment);
                            }
                        };

                    fulfillmentOrderSubscription.SubscribeToChannels("NewOrder"); //blocking
                }
            });
		}

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}