using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace OrdersDemo.ServiceModel
{
    [Route("/Orders", "GET")]
    public class Order : IReturn<Order>
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
