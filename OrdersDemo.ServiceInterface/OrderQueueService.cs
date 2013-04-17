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
                return con.GetAllEntriesFromHash("urn:OrdersInQueue")
                    .Select(x => JsonSerializer.DeserializeFromString<OrderInQueue>(x.Value))
                    .ToList();
            }
        }

        public object Post(OrderInQueue request)
        {
            using (var con = RedisClientsManager.GetClient())
            {
                request.CreatedDate = DateTime.Now;
                con.SetEntryInHash("urn:OrdersInQueue", request.OrderId.ToString(), request.ToJson());

                return "Item Succesfully Added";
            }
        }

        public object Put(OrderInQueue request)
        {
            using (var con = RedisClientsManager.GetClient())
            {
                //not the right way to do this
                var orderToUpdateJson = con.GetValueFromHash("urn:OrdersInQueue", request.OrderId.ToString());

                if (!String.IsNullOrEmpty(orderToUpdateJson))
                {
                    var orderToUpdate = JsonSerializer.DeserializeFromString<OrderInQueue>(orderToUpdateJson);
                    orderToUpdate.Status = request.Status;
                    orderToUpdate.Fulfiller = request.Fulfiller;
                    con.SetEntryInHash("urn:OrdersInQueue", orderToUpdate.OrderId.ToString(), orderToUpdate.ToJson());
                    return "Update Sucessful";
                }

                return "Entry Not Found";
            }   
        }
    }
}