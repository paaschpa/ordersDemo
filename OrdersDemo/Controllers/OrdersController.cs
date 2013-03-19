using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrdersDemo.App_Start;
using OrdersDemo.Services;

namespace OrdersDemo.Controllers
{
    public class OrdersController : Controller
    {
        //
        // GET: /Orders/
        public ActionResult Index()
        {
            return View();
        }
    }
}
