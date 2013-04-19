using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrdersDemo.Models;
using ServiceStack.Mvc;

namespace OrdersDemo.Controllers
{
    public class OrdersController : ServiceStackController
    {
        public ActionResult Index()
        {
            //Feel like front loading some static data
            var itemSets = ArcherItems.Get().Select((itm, i) => new {Value = itm, Index = i}).GroupBy(x => x.Index/3);
            return View(itemSets);
        }

        public ActionResult Queue()
        {
            return View();
        }
    }
}
