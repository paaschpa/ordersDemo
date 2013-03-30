using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using OrdersDemo.Models;
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
            Plugins.Add(new AuthFeature(
                    () => new AuthUserSession(),
                    new IAuthProvider[] { new MyCredentialsAuthProvider() }
                ) {HtmlRedirect = null});

            var dataFilePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\data.db";
		    container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(dataFilePath, SqliteDialect.Provider));

		    var userRep = new OrmLiteAuthRepository(container.Resolve<IDbConnectionFactory>());
            userRep.CreateMissingTables();
            foreach(var user in DummyUserAccounts.GetDummyAccounts())
            {
                if(userRep.GetUserAuthByUserName(user.UserName) == null)
                    userRep.CreateUserAuth(new UserAuth {UserName = user.UserName}, user.Password);
            }
		    container.Register<IUserAuthRepository>(userRep);

            container.Register<IRedisClientsManager>(new BasicRedisClientManager("localhost:6379"));

            container.Register<ICacheClient>(c =>(ICacheClient)c.Resolve<IRedisClientsManager>().GetCacheClient());
		    using (var con = AppHostBase.Resolve<IDbConnectionFactory>().OpenDbConnection())
		    {
                con.CreateTable<Order>();
                con.CreateTable<Fulfillment>();
		    }

		    //Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));


            //Fulfillment is Listening for New Orders messages
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var redisConsumer = AppHost.Resolve<IRedisClientsManager>().GetClient())
                using (var fulfillmentOrderSubscription = redisConsumer.CreateSubscription())
                {
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

            //OrderQueue is Listening for New Orders messages
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var redisConsumer = AppHost.Resolve<IRedisClientsManager>().GetClient())
                using (var OrderQueueSubscription = redisConsumer.CreateSubscription())
                {
                    OrderQueueSubscription.OnSubscribe = channel =>
                    {
                        Console.WriteLine("OrderQueue listening for orders on channel {0}", channel);
                    };
                    OrderQueueSubscription.OnUnSubscribe = channel =>
                    {
                        Console.WriteLine("OrderQueue UnSubscribed from '{0}'", channel);
                    };

                    OrderQueueSubscription.OnMessage = (channel, msg) =>
                    {
                        var createOrderRequest = msg.FromJson<CreateOrder>();
                        var createOrderInQueue = new OrderInQueue
                        {
                            CustomerName = createOrderRequest.CustomerName,
                            Status = "New"
                        };
                        using (var service = AppHostBase.Resolve<OrderQueueService>())
                        {
                            service.Post(createOrderInQueue);
                        }
                    };

                    OrderQueueSubscription.SubscribeToChannels("NewOrder"); //blocking
                }
            });
		}

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}