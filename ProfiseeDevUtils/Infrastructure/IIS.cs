using System.Diagnostics;

namespace ProfiseeDevUtils.Infrastructure
{
    public class IIS
    {
        public ILogger Logger { get; set; }

        public IIS(bool? quiet = false)
        {
            this.Logger = new Logger(quiet);
        }

        public void Start()
        {
            this.Logger.Inform("Starting IIS");
            var procInfo = new ProcessStartInfo("iisreset", "/start");
            procInfo.RedirectStandardOutput = true;
            this.StartProcess(procInfo);
            this.Logger.Inform("IIS started");
        }

        public void Stop()
        {
            this.Logger.Inform("Stopping IIS");
            var procInfo = new ProcessStartInfo("iisreset", "/stop");
            procInfo.RedirectStandardOutput = true;
            this.StartProcess(procInfo);
            this.Logger.Inform("IIS stopped");
        }

        public virtual string StartProcess(ProcessStartInfo processStartInfo)
        {
            var process = Process.Start(processStartInfo);
            var response = process?.StandardOutput.ReadToEnd() ?? string.Empty;
            process?.WaitForExit();
            return response;
        }
    }
}
