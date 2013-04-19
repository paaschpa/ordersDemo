using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    [Authenticate]
    public class FulfillmentService : OrdersDemoServiceBase
    {
        public List<Fulfillment> Get(Fulfillment request)
        {
            var fulfillments = DbConnExec((con) => con.Select<Fulfillment>(f => f.OrderBy(x => x.Id)));
            return fulfillments;
        }

        public Fulfillment Post(CreateFulfillment request)
        {
            var newFulfilllment = request.TranslateTo<Fulfillment>();
            newFulfilllment.Status = "New";

            DbConnExec((con) =>
                {
                    con.Insert<Fulfillment>(newFulfilllment);
                    newFulfilllment.Id = (int)con.GetLastInsertId();
                });

            return newFulfilllment;
        }

        public Fulfillment Put(UpdateFulfillment request)
        {
            Fulfillment fulfillmentToUpdate = null;
            DbConnExecTransaction((con) =>
                {
                    fulfillmentToUpdate = con.GetById<Fulfillment>(request.Id);
                    fulfillmentToUpdate.Status = request.Status;
                    fulfillmentToUpdate.Fulfiller = base.SessionAs<AuthUserSession>().UserName;
                    con.Update<Fulfillment>(fulfillmentToUpdate);
                });
   
                //publish message
            RedisExec((redisCon) => redisCon.PublishMessage("FulfillmentUpdate", fulfillmentToUpdate.ToJson()));

            return fulfillmentToUpdate;
        }
    }
}