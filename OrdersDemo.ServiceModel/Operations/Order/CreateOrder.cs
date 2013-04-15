using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel.Operations
{
    [Route("/Orders", "POST")]
    public class CreateOrder : IReturn<Order>
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}
