using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrdersDemo.Models
{
    public static class DummyUserAccounts
    {
        public static List<DummyUserAccount> GetDummyAccounts()
        {
           return new List<DummyUserAccount>
                                             {
                                                 new DummyUserAccount
                                                     {
                                                         UserName = "TestUser",
                                                         Password = "Password"
                                                     }
                                             };
        }

    }
    
    public class DummyUserAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}