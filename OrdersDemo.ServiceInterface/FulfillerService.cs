using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;

namespace OrdersDemo.ServiceInterface
{
    [Authenticate]
    public class FulfillmentService : Service
    {
        public IDbConnectionFactory DbConnectionFactory { get; set; }

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

        public Fulfillment Put(Fulfillment request)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var updatedFulfillment = request.TranslateTo<Fulfillment>();
                con.Update<Fulfillment>(updatedFulfillment);
                return updatedFulfillment;
            }
        }
    }
}