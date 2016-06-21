using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using WcfServiceTest01;

namespace WinHostingWcfServiceTest01
{
    public partial class Service1 : ServiceBase
    {
        internal static ServiceHost myServiceHost = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
            }
            myServiceHost = new ServiceHost(typeof(Calc));
            myServiceHost.Open();
            this.EventLog.WriteEntry(string.Format("Communication State: {0}", myServiceHost.State), EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
                myServiceHost = null;
            }
        }
    }
}
