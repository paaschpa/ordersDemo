using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel
{
    [Route("/WaitingFulfillment", "GET")]
    public class OrderInQueue
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Fulfiller { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
