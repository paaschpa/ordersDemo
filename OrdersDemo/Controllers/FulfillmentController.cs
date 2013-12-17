using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack;
using ServiceStack.Mvc;

namespace OrdersDemo.Controllers
{
    //*If you dont add <AuthUserSession> (or your subclass of it) this.AuthSession  
    public class FulfillmentController : ServiceStackController<AuthUserSession> 
    {
        [Authenticate]
        public ActionResult Index()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.UserName = this.AuthSession.UserName;
            return View(viewModel);
        }

        public ActionResult Leaders()
        {
            return View();
        }

        public override ActionResult AuthenticationErrorResult
        {
            get
            {
                if (this.AuthSession == null || this.AuthSession.IsAuthenticated == false)
                {
                    return Redirect("~/Home/Login");
                }
                return base.AuthenticationErrorResult;
            }
        }
    }
}
