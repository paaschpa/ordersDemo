﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Mvc;

namespace OrdersDemo.Controllers
{
    public class OrdersController : ServiceStackController
    {
        //
        // GET: /Orders/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Queue()
        {
            return View();
        }
    }
}