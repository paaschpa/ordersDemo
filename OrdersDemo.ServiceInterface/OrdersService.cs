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
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    public class OrdersService : OrdersDemoServiceBase
    {
        public List<Order> Get(Order request)
        {
            var orders = DbConnExec((con) => con.Select<Order>(o => o.OrderBy(x => x.Id)));
            return orders;
        }

        public Order Post(CreateOrder request)
        {
            var newOrder = request.TranslateTo<Order>();
            newOrder.Status = "New";
            DbConnExecTransaction((con) =>
                {
                    con.Insert(newOrder);
                    newOrder.Id = (int) con.GetLastInsertId();
                });

            RedisExec((redisCon) => redisCon.PublishMessage("NewOrder", newOrder.ToJson()));

            return newOrder;
        }
    }
}