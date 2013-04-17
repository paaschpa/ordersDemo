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

        public object Put(OrderInQueue request)
        {
            using (var con = RedisClientsManager.GetClient())
            {
                //not the right way to do this
                var o = con.GetAllItemsFromList("urn:OrdersInQueue");
                var queuedOrders = con.GetAllItemsFromList("urn:OrdersInQueue").Select(x => x.To<OrderInQueue>());
                var orderToUpdate =
                    queuedOrders.FirstOrDefault(x => x.ItemName == request.ItemName && x.Quantity == request.Quantity);

                if (orderToUpdate != null)
                {
                    orderToUpdate.Status = request.Status;
                    con.Set("urn:OrdersInQueue", queuedOrders);
                }

                return "Update Successful";
            }   
        }
    }
}