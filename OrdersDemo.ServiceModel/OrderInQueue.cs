using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace OrdersDemo.ServiceModel
{
    [Route("/OrderInQueue", "GET")]
    public class OrderInQueue
    {
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
