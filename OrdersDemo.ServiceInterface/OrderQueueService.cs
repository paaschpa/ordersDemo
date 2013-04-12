using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace OrdersDemo.Services
{
    [Route("/OrderInQueue")]
    public class OrderInQueue
    {
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateOrderInQueueResponse
    {
        public CreateOrderInQueueResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        public ResponseStatus ResponseStatus { get; set; }
    }

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
                return new CreateOrderInQueueResponse();
            }
        }
    }
}