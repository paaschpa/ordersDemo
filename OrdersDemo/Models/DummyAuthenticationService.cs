using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrdersDemo.Models
{
    public static class DummyAuthenticationService
    {
        public static List<DummyUserAccount> DummyUserAccounts = new List<DummyUserAccount>
                                                                     {
                                                                         new DummyUserAccount
                                                                             {
                                                                                 UserName = "TestUser",
                                                                                 Password = "Password"
                                                                             }
                                                                     };

        public static bool Authenticate(string userName, string password)
        {
            return DummyUserAccounts.Any(x => x.UserName == userName && x.Password == password);
        }
    }
}