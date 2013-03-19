using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrdersDemo.App_Start;
using OrdersDemo.Services;

namespace OrdersDemo.Controllers
{
    public class FulfillmentController : Controller
    {
        //
        // GET: /Fulfillment/

        public ActionResult Index()
        {
            return View();
        }
    }
}
