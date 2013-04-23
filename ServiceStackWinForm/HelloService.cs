using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace ServiceStackWinForm
{
    [Description("ServiceStack's Hello World web service.")]
    [Route("/hello")]
    [Route("/hello/{Name*}")]
    public class Hello 
    {
        public string Name { get; set; }
    }

    public class HelloService : Service
    {
        public string Any(Hello request)
        {
            foreach (var fm in Application.OpenForms)
            {
                if (((Form)fm).Name == "Main") //Find the form by name
                {
                    ((Main)fm).Invoke(new Action<Form, string>(UpdateLog), fm, request.Name); //Can only make changes to WinFrom controls from master thread so 'delegating' method back to form
                }
            }
            return "Hello, " + request.Name;
        }

        public void UpdateLog(Form fm, string name)
        {
            ((Main)fm).AccessCount++;
            ((Main)fm).UpdateLog(name);
        }
    }
}
