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
                { "clone", (a, b) => Clone(a, b) },
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

        public void Clone(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            if (Directory.Exists(repoPath))
            {
                Console.WriteLine($"Repo {repoName} has already been created at {repoPath}");
                return;
            }

            var gitUrl = $"https://profisee.visualstudio.com/Products/_git/{repoName}";
            Console.WriteLine($"cloning {gitUrl}");
            var processInfo = new ProcessStartInfo("git", $"clone {gitUrl}");
            processInfo.WorkingDirectory = repoPath;
            Process.Start(processInfo)?.WaitForExit();
        }

        public void Pull(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            Console.WriteLine($"pulling latest for {repoName}");
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
