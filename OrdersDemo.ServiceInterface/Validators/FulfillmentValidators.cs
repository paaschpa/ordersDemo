using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.CacheAccess;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace OrdersDemo.ServiceInterface.Validators
{
    public class UpdatefillmentValidator : AbstractValidator<UpdateFulfillment>
    {
        public ICacheClient CacheClient { get; set; }

        public UpdatefillmentValidator()
        {
            RuleFor(f => f.Fulfiller).Must(ValidUpdater).WithMessage("Not yours to complete!");
        }

        public bool ValidUpdater(UpdateFulfillment instance, string fulfiller)
        {
            var key = SessionFeature.GetSessionKey();
            var currentUser = CacheClient.Get<AuthUserSession>(key);
            if (instance.Status == "Completed" && fulfiller != currentUser.UserName)
            {
                return false;
            }
            return true;
        }
    }
}
