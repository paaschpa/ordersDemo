using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel.Operations
{
    [Route("/Fulfillment", "POST")]
    public class CreateFulfillment : IReturn<Fulfillment>
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}
