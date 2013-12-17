using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
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
            var newOrder = request.ConvertTo<Order>();
            newOrder.Status = "New";
            DbConnExecTransaction((con) =>
                {
                    con.Insert(newOrder);
                    newOrder.Id = (int) con.LastInsertId();
                });

            RedisExec((redisCon) => redisCon.PublishMessage("NewOrder", newOrder.ToJson()));

            return newOrder;
        }
    }
}