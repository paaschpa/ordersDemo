using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OrdersDemo.ServiceModel;
using OrdersDemo.ServiceModel.Operations;
using ServiceStack;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace OrdersDemo.ServiceInterface
{
    [Authenticate]
    public class FulfillmentService : OrdersDemoServiceBase
    {
        public List<Leader> Get(Leaders request)
        {
            var sql = @"Select Fulfiller as Name, Count(Id) as Score From Fulfillment 
                        Where Status = 'Completed' Group By Fulfiller
                        Order by Count(Id) desc";
            var leaders = DbConnExec<List<Leader>>((con) => con.Select<Leader>(sql));

            return leaders;
        }

        public List<Fulfillment> Get(Fulfillment request)
        {
            var ev = OrmLiteConfig.DialectProvider.SqlExpression<Fulfillment>();
            ev.Where(f => f.Status != "Completed").OrderBy(f => f.Id);

            var fulfillments = DbConnExec((con) => con.Select<Fulfillment>(ev));
            return fulfillments;
        }

        public Fulfillment Post(CreateFulfillment request)
        {
            var newFulfilllment = request.ConvertTo<Fulfillment>();
            newFulfilllment.Status = "New";

            DbConnExec((con) =>
                {
                    var newId = con.Insert<Fulfillment>(newFulfilllment, true);
                    newFulfilllment.Id = (int) newId;
                });

            return newFulfilllment;
        }

        public Fulfillment Put(UpdateFulfillment request)
        {
            Fulfillment fulfillmentToUpdate = null;
            DbConnExecTransaction((con) =>
                {
                    fulfillmentToUpdate = con.LoadSingleById<Fulfillment>(request.Id);
                    if (request.Status == "Start" && !String.IsNullOrEmpty(fulfillmentToUpdate.Fulfiller))
                    {
                        throw new Exception("Already Started!");
                    }
                    fulfillmentToUpdate.Status = request.Status;
                    fulfillmentToUpdate.Fulfiller = base.SessionAs<AuthUserSession>().UserName;
                    con.Update<Fulfillment>(fulfillmentToUpdate);
                });

            //Refresh FulfillmentGrid
            var hub = GlobalHost.ConnectionManager.GetHubContext("FulfillmentGridHub");
            if (hub != null)
            {
                hub.Clients.All.updateGrid(fulfillmentToUpdate);
            }

            //Publish Message
            RedisExec((redisCon) => redisCon.PublishMessage("FulfillmentUpdate", fulfillmentToUpdate.ToJson()));


            return fulfillmentToUpdate;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}