using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrdersDemo.App_Start;
using OrdersDemo.Services;
using ServiceStack.Mvc;
using ServiceStack.ServiceInterface;

namespace OrdersDemo.Controllers
{
    public class FulfillmentController : ServiceStackController
    {
        //
        // GET: /Fulfillment/
        [Authenticate]
        public ActionResult Index()
        {
            return View();
        }
    }
}
