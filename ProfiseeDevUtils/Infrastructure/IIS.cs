using System.Diagnostics;

namespace ProfiseeDevUtils.Infrastructure
{
    public class IIS
    {
        public void Start()
        {
            var procInfo = new ProcessStartInfo("iisreset", "/start");
            this.StartProcess(procInfo);
        }

        public void Stop()
        {
            var procInfo = new ProcessStartInfo("iisreset", "/stop");
            this.StartProcess(procInfo);
        }

        public void Reset()
        {
            var procInfo = new ProcessStartInfo("iisreset");
            this.StartProcess(procInfo);
        }

        public virtual void StartProcess(ProcessStartInfo processStartInfo)
        {
            Process.Start(processStartInfo)?.WaitForExit();
        }
    }
}
