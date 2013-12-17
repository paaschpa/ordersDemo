using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;

namespace OrdersDemo.ServiceInterface.Validators
{
    public class UpdatefillmentValidator : AbstractValidator<UpdateFulfillment>
    {
        public ICacheClient CacheClient { get; set; }
        public IDbConnectionFactory DbConnectionFactory { get; set; }

        public UpdatefillmentValidator()
        {
            RuleFor(f => f.Fulfiller).Must(ValidUpdater).WithMessage("Not yours to complete!");
            RuleFor(f => f.Fulfiller).Must(ValidateNumberFulFilling).WithMessage("At Max open fulfillments!");
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

        public bool ValidateNumberFulFilling(UpdateFulfillment instance, string fulfiller)
        {
            using (var con = DbConnectionFactory.OpenDbConnection())
            {
                var numberOfFulfillments =
                    con.SqlScalar<int>("Select count(Id) From Fulfillment Where Status <> 'Completed' And Fulfiller = ?",
                                    new {fulfiller = fulfiller});
                if (instance.Status == "Start")
                {
                    numberOfFulfillments += 1; //add one to limit 'active working' fulfillments to 3 per fulfiller
                }
                if (numberOfFulfillments >= 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
