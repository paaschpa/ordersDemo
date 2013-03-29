using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrdersDemo.App_Start;
using OrdersDemo.Services;
using ServiceStack.Mvc;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace OrdersDemo.Controllers
{
    public class FulfillmentController : ServiceStackController<AuthUserSession>
    {
        [Authenticate]
        public ActionResult Index()
        {
            return View();
        }

        public override ActionResult AuthenticationErrorResult
        {
            get
            {
                if (this.AuthSession == null || this.AuthSession.IsAuthenticated == false)
                {
                    return Redirect("~/Home");
                }
                return base.AuthenticationErrorResult;
            }
        }
    }
}
