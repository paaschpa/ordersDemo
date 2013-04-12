using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrdersDemo.ServiceModel;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    public class OrderQueueService : Service
    {
        public IRedisClientsManager RedisClientsManager { get; set; }

        public List<OrderInQueue> Get (OrderInQueue request)
        {
            using (var con = RedisClientsManager.GetClient())
            {
                return con.GetAllItemsFromList("urn:OrdersInQueue")
                    .Select(x => JsonSerializer.DeserializeFromString<OrderInQueue>(x))
                    .ToList();
            }
        }

        public object Post(OrderInQueue request)
        {
            using (var con = RedisClientsManager.GetClient())
            {
                request.CreatedDate = DateTime.Now;
                con.AddItemToList("urn:OrdersInQueue", request.ToJson());

                return "Item Succesfully Added";
            }
        }
    }
}