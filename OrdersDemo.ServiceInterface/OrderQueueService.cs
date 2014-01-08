using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    public class OrderQueueService : OrdersDemoServiceBase
    {
        public List<OrderInQueue> Get (OrderInQueue request)
        {
            var ordersInQueue = RedisExec((redisCon) => redisCon.GetAllEntriesFromHash("urn:OrdersInQueue")
                                            .Select(x => x.Value.FromJson<OrderInQueue>())
                                            .OrderBy(x => x.CreatedDate).ToList());

            return ordersInQueue;
        }

        public object Post(OrderInQueue request)
        {
            request.CreatedDate = DateTime.Now;
            RedisExec(
                (redisCon) => redisCon.SetEntryInHash("urn:OrdersInQueue", request.OrderId.ToString(), request.ToJson()));
            
            return "Item Succesfully Added";
        }

        public object Put(UpdateOrderInQueue request)
        {
            var result = RedisExec((redisCon) =>
                {
                    var orderToUpdateJson = redisCon.GetValueFromHash("urn:OrdersInQueue", request.OrderId.ToString());

                    if (!String.IsNullOrEmpty(orderToUpdateJson))
                    {
                        var orderToUpdate = JsonSerializer.DeserializeFromString<OrderInQueue>(orderToUpdateJson);
                        if (request.Status == "Completed")
                        {
                            redisCon.RemoveEntryFromHash("urn:OrdersInQueue", orderToUpdate.OrderId.ToString());
                            return "Entry Removed";
                        }
                        orderToUpdate.Status = request.Status;
                        orderToUpdate.Fulfiller = request.Fulfiller;
                        redisCon.SetEntryInHash("urn:OrdersInQueue", orderToUpdate.OrderId.ToString(), orderToUpdate.ToJson());
                        return "Update Successful";
                    }
                    return "Entry Not Found";
                });

            return result;
        }   
    }
}