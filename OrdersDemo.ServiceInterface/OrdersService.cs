using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    public class OrdersService : Service
    {
        public IDbConnectionFactory dbConn { get; set; }
        public IRedisClientsManager redisClientManager { get; set; }

        public List<Order> Get(Order request)
        {
            using(var conn = dbConn.OpenDbConnection())
            {
                var orders = conn.Select<Order>(o => o.OrderBy(x => x.Id));
                return orders;
            }
        }

        public Order Post(CreateOrder request)
        {
            using (var conn = dbConn.OpenDbConnection())
            {
                var newOrder = request.TranslateTo<Order>();
                newOrder.Status = "New";
                conn.Insert(newOrder);
                newOrder.Id = (int)conn.GetLastInsertId();
                //publish message
                using (var redisClient = redisClientManager.GetClient())
                {
                    redisClient.PublishMessage("NewOrder", request.ToJson());
                }
                //Alert connections
                var hub = GlobalHost.ConnectionManager.GetHubContext("GridHub");
                if (hub != null)
                {
                    hub.Clients.All.refreshGrid("newOrder");
                }
                return newOrder;
            }
        }
    }
}