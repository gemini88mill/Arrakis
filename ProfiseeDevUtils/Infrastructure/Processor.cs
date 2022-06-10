using System.Diagnostics;

namespace ProfiseeDevUtils.Infrastructure
{
    public class Processor : IProcessor
    {
        private Process? process;
        public StreamReader? StandardOutput { get; set; }

        public IProcessor Start(ProcessStartInfo processStartInfo)
        {
            this.process = Process.Start(processStartInfo);
            this.StandardOutput = process?.StandardOutput;
            return this;
        }

        public void WaitForExit()
        {
            this.process?.WaitForExit();
        }
    }
}
