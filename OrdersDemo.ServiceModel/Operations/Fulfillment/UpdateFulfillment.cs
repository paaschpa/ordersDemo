using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel.Operations
{
    [Route("/Fulfillment", "POST")]
    public class UpdateFulfillment
    {
        public int FulfillmentId { get; set; }
        public string Status { get; set; }
    }
}
