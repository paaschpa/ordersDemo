using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStackWinForm
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost() : base("Form HttpListener", typeof(AppHost).Assembly) { }

        public override void Configure(Funq.Container container)
        {

        }
    }
}
