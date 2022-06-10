using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProfiseeDevUtils.Init
{
    public class PreReqs
    {
        private readonly bool quiet;

        public ILogger Logger { get; set; } = new Logger();

        public PreReqs(bool? quiet)
        {
            this.quiet = quiet ?? false;
        }

        public bool Cheq()
        {
            var hasDotNet3_1 = checkDotNet(3, 1);
            var hasDotNet6_0 = checkDotNet(6, 0);
            return hasDotNet3_1 && hasDotNet6_0;
        }

        public bool checkDotNet(int majorVersion, int minorVersion)
        {
            ProcessStartInfo procInfo = new ProcessStartInfo("dotnet", "--list-runtimes");
            procInfo.RedirectStandardOutput = true;
            var process = Process.Start(procInfo);
            var response = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            int count = new Regex(@$"\s{majorVersion}\.{minorVersion}\.").Matches(response).Count;
            log($"Found {count} versions of dotnet {majorVersion}.{minorVersion}");

            var hasEnoughVersions = count > 1;
            if (!hasEnoughVersions)
            {
                Console.WriteLine($"You are missing one or more runtimes of dotnet {majorVersion}.{minorVersion}");
            }

            return hasEnoughVersions;
        }

        private void log(string message)
        {
            if (this.quiet)
            {
                return;
            }

            this.Logger.WriteLine(message);
        }
    }
}
