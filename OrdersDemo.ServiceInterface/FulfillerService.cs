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
    public class FulfillmentService : Service
    {
        public IDbConnectionFactory DbConnectionFactory { get; set; }
        public IRedisClientsManager redisClientManager { get; set; }

        public List<Fulfillment> Get(Fulfillment request)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var fulfillments = con.Select<Fulfillment>(f => f.OrderBy(x => x.Id));

                return fulfillments;
            }
        }

        public Fulfillment Post(CreateFulfillment request)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var newFulfilllment = request.TranslateTo<Fulfillment>();
                newFulfilllment.Status = "New";

                con.Insert<Fulfillment>(newFulfilllment);

                return newFulfilllment;
            }
        }

        public Fulfillment Put(UpdateFulfillment request)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var fulfillmentToUpdate = con.GetById<Fulfillment>(request.Id);
                fulfillmentToUpdate.Status = request.Status;
                fulfillmentToUpdate.Fulfiller = base.SessionAs<AuthUserSession>().UserName;
                con.Update<Fulfillment>(fulfillmentToUpdate);

                //publish message
                using (var redisClient = redisClientManager.GetClient())
                {
                    redisClient.PublishMessage("FulfillmentUpdate", fulfillmentToUpdate.ToJson());
                }

                return fulfillmentToUpdate;
            }
        }
    }
}