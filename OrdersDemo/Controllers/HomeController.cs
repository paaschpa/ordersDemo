using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OrdersDemo.Models;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Mvc;

namespace OrdersDemo.Controllers
{
    public class HomeController : ServiceStackController
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
                var apiAuthService = AppHostBase.Instance.Resolve<AuthenticateService>();
                apiAuthService.Request = System.Web.HttpContext.Current.ToRequest();
                var apiResponse = apiAuthService.Authenticate(new Authenticate
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
            var apiAuthService = AppHostBase.Instance.Resolve<AuthenticateService>();
            apiAuthService.Request = System.Web.HttpContext.Current.ToRequest();
            apiAuthService.Post(new Authenticate() { provider = "logout" });
            //forms logout
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

    }
}
