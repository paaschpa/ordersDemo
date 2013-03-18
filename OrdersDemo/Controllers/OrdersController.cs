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
            using (var ordersService = AppHost.Resolve<OrdersService>())
            {
                var model = ordersService.Get(new Order()).Orders;
                return View(model);
            }
        }

    }
}
