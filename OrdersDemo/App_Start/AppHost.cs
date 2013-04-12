using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using OrdersDemo.Models;
using OrdersDemo.ServiceInterface;
using OrdersDemo.ServiceInterface.Subscribers;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
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
                    new IAuthProvider[] { new CredentialsAuthProvider() }
                ) {HtmlRedirect = null});

            var dataFilePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\data.db";
		    container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(dataFilePath, SqliteDialect.Provider));

            var userRep = new OrmLiteAuthRepository(container.Resolve<IDbConnectionFactory>());
		    container.Register<IUserAuthRepository>(userRep);
            container.Register<IRedisClientsManager>(new BasicRedisClientManager("localhost:6379"));
            container.Register<ICacheClient>(c =>(ICacheClient)c.Resolve<IRedisClientsManager>().GetCacheClient());

		    //Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));

            //https://github.com/ServiceStack/ServiceStack.Redis/wiki/RedisPubSub
            //start threads that subscribe to Redis channels for Pub/Sub
            new OrderSubscribers().StartSubscriberThreads(container);

            //https://github.com/ServiceStack/ServiceStack/wiki/Authentication-and-authorization#userauth-persistence---the-iuserauthrepository
            //Use ServiceStacks authentication/authorization persistence
            userRep.CreateMissingTables(); //Create missing Auth
            
            //Create Tables for the demo
            using (var con = AppHostBase.Resolve<IDbConnectionFactory>().OpenDbConnection())
		    {
                con.CreateTable<Order>();
                con.CreateTable<Fulfillment>();
		    }

            //Create dummy user accounts (TestUser/Password)
            foreach(var user in DummyUserAccounts.GetDummyAccounts())
            {
                if(userRep.GetUserAuthByUserName(user.UserName) == null)
                    userRep.CreateUserAuth(new UserAuth {UserName = user.UserName}, user.Password);
            }
		}

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}