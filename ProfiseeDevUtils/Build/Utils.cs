using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;
using System.ServiceProcess;

namespace ProfiseeDevUtils.Build
{
    public class Utils
    {
        public ILogger Logger { get; set; }

        public Utils(bool? quiet = false)
        {
             this.Logger = new Logger(quiet);
        }

        public void TurnOnService(string name)
        {
            ServiceController service = new ServiceController(name);
            if(service.Status == ServiceControllerStatus.Running)
            {
                this.Logger.Inform("Service is already started");
                return;
            }
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
            this.Logger.Inform("Service has successfully started");
        }

        public void TurnOffService(string name)
        {
            ServiceController service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                this.Logger.Inform("Service is already stopped");
                return;
            }
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
            this.Logger.Inform("Service has successfully stopped");
        }

        public string[] GetFilesByType(string root, string type)
        {
            return Directory.GetFiles(root, $"*.{type}", SearchOption.AllDirectories);
        }

        public string[] GetFoldersByName(string root, string dirName)
        {
            return Directory.GetDirectories(root, dirName, SearchOption.AllDirectories);
        }

        public string? GetFolderByFileName(string root, string fileName) => Directory.GetFiles(root, $"{fileName}.*", SearchOption.AllDirectories).FirstOrDefault();


        public List<string> GetDefaultSlns(string root)
        {
            var gits = Directory.GetDirectories(root);
            List<string> slns = new List<string>();
            foreach (var g in gits)
            {
               slns.AddRange(Directory.GetFiles(g, $"*.sln"));
            }

            return slns;
        }

        public void IISStart()
        {
            Process.Start("iisstart.bat");
        }

        public void IISStop()
        {
            Process.Start("iisstop.bat");
        }

    }
}
