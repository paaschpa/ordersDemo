using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    [Authenticate]
    public class FulfillmentService : OrdersDemoServiceBase
    {
        public List<Leader> Get(Leaders request)
        {
            var leaders = DbConnExec<List<Leader>>((con) => con
                .Select<Leader>(@"Select Fulfiller as Name, Count(Id) as Score From Fulfillment Where Status = 'Completed' Group By Fulfiller"));

            return leaders;
        }

        public List<Fulfillment> Get(Fulfillment request)
        {
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<Fulfillment>();
            ev.Where(f => f.Status != "Completed").OrderBy(f => f.Id);

            var fulfillments = DbConnExec((con) => con.Select<Fulfillment>(ev));
            return fulfillments;
        }

        public Fulfillment Post(CreateFulfillment request)
        {
            var newFulfilllment = request.TranslateTo<Fulfillment>();
            newFulfilllment.Status = "New";

            DbConnExec((con) =>
                {
                    con.Insert<Fulfillment>(newFulfilllment);
                    newFulfilllment.Id = (int)con.GetLastInsertId();
                });

            return newFulfilllment;
        }

        public Fulfillment Put(UpdateFulfillment request)
        {
            Fulfillment fulfillmentToUpdate = null;
            DbConnExecTransaction((con) =>
                {
                    fulfillmentToUpdate = con.GetById<Fulfillment>(request.Id);
                    if (request.Status == "Start" && !String.IsNullOrEmpty(fulfillmentToUpdate.Fulfiller))
                    {
                        throw new Exception("Already Started!");
                    }
                    fulfillmentToUpdate.Status = request.Status;
                    fulfillmentToUpdate.Fulfiller = base.SessionAs<AuthUserSession>().UserName;
                    con.Update<Fulfillment>(fulfillmentToUpdate);
                });
   
                var hub = GlobalHost.ConnectionManager.GetHubContext("GridHub");
                if (hub != null)
                {
                    hub.Clients.All.updateGrid(fulfillmentToUpdate);
                }

            return fulfillmentToUpdate;
        }
    }
}