using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.FluentValidation;

namespace OrdersDemo.ServiceInterface.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrder>
    {
        public CreateOrderValidator()
        {
            RuleFor(o => o.Quantity).InclusiveBetween(1, 100);
            RuleFor(o => o.CustomerFirstName).NotEmpty();
            RuleFor(o => o.CustomerLastName).NotEmpty();
        }
    }
}
