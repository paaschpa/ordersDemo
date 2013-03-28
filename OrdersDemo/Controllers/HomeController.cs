using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OrdersDemo.Models;
using ServiceStack;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

namespace OrdersDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string userName, string password)
        {
            try
            {
                var apiAuthService = AppHostBase.Resolve<AuthService>();
                apiAuthService.RequestContext = System.Web.HttpContext.Current.ToRequestContext();
                var apiResponse = apiAuthService.Authenticate(new Auth
                {
                    UserName = userName,
                    Password = password,
                    RememberMe = false
                });

                return RedirectToAction("Index", "Fulfillment");
            }
            catch (Exception)
            {
               ModelState.AddModelError("AuthenticationError", "The user name or password provided is incorrect.");
                return View();
            }
        }

        public ActionResult LogOut()
        {
            //api logout
            var apiAuthService = AppHostBase.Resolve<AuthService>();
            apiAuthService.RequestContext = System.Web.HttpContext.Current.ToRequestContext();
            apiAuthService.Post(new Auth() { provider = "logout" });
            //forms logout
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
