using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Configuration.Install;

namespace HttpServerTestService
{
    public partial class HttpServerTestService : ServiceBase
    {
        static EventLog installEvent;
        private static bool IsInstalled()
        {
            using (ServiceController controller =
                new ServiceController("HttpServerTestService"))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static bool IsRunning()
        {
            using (ServiceController controller =
                new ServiceController("HttpServerTestService"))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(ProjectInstaller).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        private static void InstallService()
        {
            if (IsInstalled()) return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller =
                new ServiceController("HttpServerTestService"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static void StopService()
        {
            if (!IsInstalled()) return;
            using (ServiceController controller =
                new ServiceController("HttpServerTestService"))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        static void Main(string[] args)
        {
            if (installEvent==null) installEvent = new EventLog("HttpServerInstallServiceLog", Environment.MachineName, "HttpServerTestServiceSource");
            installEvent.BeginInit();
            if (!EventLog.SourceExists("HttpServerTestServiceSource"))
            {
                EventLog.CreateEventSource("HttpServerTestServiceSource", "HttpServerInstallServiceLog");
            }
            installEvent.EndInit();
            if (args.Length == 0)
            {
                // Run your service normally.
                ServiceBase[] ServicesToRun = new ServiceBase[] { new HttpServerTestService() };
                Run(ServicesToRun);
            }
            else if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "-ir":
                        InstallService();
                        StartService();
                        break;
                    case "-i":
                        InstallService();
                        break;
                    case "-u":
                    case "-su":
                        StopService();
                        UninstallService();
                        break;
                    case "-s":
                        StopService();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        EventLog eventLog1;
        public HttpServerTestService()
        {
            InitializeComponent();
            if (eventLog1 == null) eventLog1 = new EventLog("HttpServerTestServiceLog", Environment.MachineName, "HttpServerTestServiceSource");
            if (!EventLog.SourceExists("HttpServerTestServiceSource"))
            {
                EventLog.CreateEventSource("HttpServerTestServiceSource", "HttpServerTestServiceLog");
            }
        }

        protected override void OnStart(string[] args)
        {
            if (args != null && args.Length > 0)
                eventLog1.WriteEntry(string.Format("Service Started with args: {0}", args), EventLogEntryType.Information, 1, 1);
            else
                eventLog1.WriteEntry("Service Started without args", EventLogEntryType.Information, 1, 1);
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry(string.Format("Service Stopped"), EventLogEntryType.Information, 2, 1);
            base.OnStop();
        }

        protected override void OnPause()
        {
            eventLog1.WriteEntry(string.Format("Service Paused"), EventLogEntryType.Information, 3, 1);
            base.OnPause();
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry(string.Format("Service Continued"), EventLogEntryType.Information, 4, 1);
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry(string.Format("Service Shutdown"), EventLogEntryType.Information, 5, 1);
            base.OnShutdown();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            eventLog1.WriteEntry(string.Format("Service Power Event: {0}", powerStatus), EventLogEntryType.Information, 6, 1);
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            eventLog1.WriteEntry(string.Format("Service Session({0:D8}) Changed: {1}", changeDescription.SessionId, changeDescription.Reason), EventLogEntryType.Information, 7, 1);
            base.OnSessionChange(changeDescription);
        }

        protected override void OnCustomCommand(int command)
        {
            eventLog1.WriteEntry(string.Format("Service Custom Command: {0}", command), EventLogEntryType.Information, 8, 1);
            base.OnCustomCommand(command);
        }
    }
}