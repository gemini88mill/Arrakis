using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProfiseeDevUtils.Init
{
    public class PreReqs
    {
        public ILogger Logger { get; set; }

        public PreReqs(bool? quiet)
        {
            this.Logger = new Logger(quiet);
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
            var response = StartProcess(procInfo);

            int count = new Regex(@$"\s{majorVersion}\.{minorVersion}\.").Matches(response).Count;
            this.Logger.Inform($"Found {count} versions of dotnet {majorVersion}.{minorVersion}");

            var hasEnoughVersions = count > 1;
            if (!hasEnoughVersions)
            {
                this.Logger.Err($"You are missing one or more runtimes of dotnet {majorVersion}.{minorVersion}");
            }

            return hasEnoughVersions;
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
