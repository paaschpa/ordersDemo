using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel
{
    [Route("/Fulfillment", "GET")]
    public class Fulfillment
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Fulfiller { get; set; }
        public string Status { get; set; }
    }

    public class FulfillmentUpdateResponse
    {
    }
}
