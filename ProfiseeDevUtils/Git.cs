using ProfiseeDevUtils.Init;
using System.Diagnostics;

namespace ProfiseeDevUtils
{
    public class Git
    {
        public string RootPath { get; set; }

        private string[] repos =
        {
            "auth",
            "BuildTools",
            "file-attachment",
            "governance",
            "machine-learning",
            "platform",
            "rest-api",
            "sdk",
            "Workflow"
        };

        public Git()
        {
            this.RootPath = new EnvironmentVariables(false).GetEnvVar("gitRepos") ?? @"C:\DevOps\Repos";
        }

        public void Act(string action, string repoName, string branch)
        {
            var gitCommands = new Dictionary<string, Action<string, string>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "pull", (a, b) => Pull(a, b) },
                { "status", (a, b) => Status(a, b) }
            };

            if (!gitCommands.ContainsKey(action))
            {
                Console.WriteLine($"Action '{action}' not found");
                return;
            }

            if (string.IsNullOrWhiteSpace(repoName))
            {
                foreach (var repo in repos)
                {
                    gitCommands[action](repo, branch);
                }

                return;
            }

            gitCommands[action](repoName, branch);
        }

        public void Pull(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            Console.WriteLine($"git pull in {repoPath}");
            var processInfo = new ProcessStartInfo("git", "pull");
            processInfo.WorkingDirectory = repoPath;
            Process.Start(processInfo)?.WaitForExit();
        }

        public void Status(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            var processInfo = new ProcessStartInfo("git", "status -bs");
            processInfo.WorkingDirectory = repoPath;
            Console.WriteLine($"-------- {repoName} --------");
            Process.Start(processInfo)?.WaitForExit();
        }
    }
}
