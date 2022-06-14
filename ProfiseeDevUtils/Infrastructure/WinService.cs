using System.ServiceProcess;

namespace ProfiseeDevUtils.Infrastructure
{
    internal class WinService
    {
        public ILogger Logger { get; set; }

        public WinService(bool? quiet = false)
        {
            this.Logger = new Logger(quiet);
        }

        public void Start(string name)
        {
            ServiceController service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Running)
            {
                this.Logger.Inform($"Service '{name}' is already started");
                return;
            }
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
            this.Logger.Inform($"Service '{name}' has successfully started");
        }

        public void Stop(string name)
        {
            ServiceController service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                this.Logger.Inform($"Service '{name}' is already stopped");
                return;
            }
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
            this.Logger.Inform($"Service '{name}' has successfully stopped");
        }
    }
}
