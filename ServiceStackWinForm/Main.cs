using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceStackWinForm
{
    public partial class Main : Form
    {
        public int AccessCount;

        public Main()
        {
            InitializeComponent();

            var appHost = new AppHost();
            appHost.Init();
            appHost.Start("http://localhost:1337/");
            UpdateLog("Started listening on: ");

            UpdateLog(string.Format("AppHost Created at {0}, listening on {1}", DateTime.Now, "localhost:1337")); 
        }

        public void UpdateLog(string data)
        {
            LogBox.Text +=  DateTime.Now.ToString("hh:mm:ss:fff") + " - " + data + "\n";
        }
    }
}
