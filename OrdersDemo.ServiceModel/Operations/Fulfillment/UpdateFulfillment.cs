using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack;

namespace OrdersDemo.ServiceModel.Operations
{
    [Route("/Fulfillment", "PUT")]
    public class UpdateFulfillment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Fulfiller { get; set; }
        public string Status { get; set; }
    }
}
