using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace OrdersDemo.Services
{
    [Route("/Fulfillment")]
    public class Fulfillment
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Fulfiller { get; set; }
        public string Status { get; set; }
    }

    public class FulfillmentResponse : IHasResponseStatus
    {
        public FulfillmentResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        public ResponseStatus ResponseStatus { get; set; }
        public List<Fulfillment> FulFillments { get; set; }
    }

    public class CreateFulfillment : IReturn<CreateFulfillmentResponse>
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateFulfillmentResponse : IHasResponseStatus
    {
        public CreateFulfillmentResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class FulfillmentService : Service
    {
        public IDbConnectionFactory DbConnectionFactory { get; set; }

        public FulfillmentResponse Get(Fulfillment reqeust)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var fulfillments = con.Select<Fulfillment>();

                return new FulfillmentResponse {FulFillments = fulfillments};
            }
        }

        public CreateFulfillmentResponse Post(CreateFulfillment request)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var newFulfilllment = request.TranslateTo<Fulfillment>();
                newFulfilllment.Status = "New";

                con.Insert<Fulfillment>(newFulfilllment);

                return new CreateFulfillmentResponse();
            }
        }
    }
}