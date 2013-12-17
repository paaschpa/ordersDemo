using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack;

namespace OrdersDemo.ServiceModel.Operations
{
    [Api("Create an Order")]
    [Route("/Orders", "POST")]
    public class CreateOrder : IReturn<Order>
    {
        [ApiMember(Name = "CustomerFirstName", Description = "First Name", ParameterType = "body", DataType = "string", IsRequired = true)]
        public string CustomerFirstName { get; set; }
       
        [ApiMember(Name = "CustomerLastName", Description = "Last Name", ParameterType = "body", DataType = "string", IsRequired = true)]
        public string CustomerLastName { get; set; }

        [ApiMember(Name = "ItemId", Description = "Item Id", ParameterType = "body", DataType = "int", IsRequired = true)]
        public int ItemId { get; set; }
       
        [ApiMember(Name = "ItemName", Description = "Name of the Item", ParameterType = "body", DataType = "string", IsRequired = true)]
        public string ItemName { get; set; }
        
        [ApiMember(Name = "Quantity", Description = "Number of Items Purchased", ParameterType = "body", DataType = "int", IsRequired = true)]
        public int Quantity { get; set; }
    }
}
