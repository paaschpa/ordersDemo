using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersDemo.ServiceModel.Operations
{
    public class UpdateOrderInQueue
    {
        public int OrderId { get; set; }
        public string Fulfiller { get; set; }
        public string Status { get; set; }
    }
}
