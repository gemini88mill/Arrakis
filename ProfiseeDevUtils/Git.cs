using ProfiseeDevUtils.Infrastructure;
using ProfiseeDevUtils.Init;
using System.Diagnostics;

namespace ProfiseeDevUtils
{
    public class Git
    {
        public ILogger Logger { get; set; } = new Logger();
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
            this.RootPath = new EnvironmentVariables(false).gitRepos ?? @"C:\DevOps\Repos";
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
                this.Logger.WriteLine($"Action '{action}' not found");
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
                this.Logger.WriteLine($"Repo {repoName} has already been created at {repoPath}");
                return;
            }

            var gitUrl = $"https://profisee.visualstudio.com/Products/_git/{repoName}";
            this.Logger.WriteLine($"cloning {gitUrl}");
            var processInfo = new ProcessStartInfo("git", $"clone {gitUrl}");
            processInfo.WorkingDirectory = repoPath;
            this.StartProcess(processInfo);
        }

        public void Pull(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            this.Logger.WriteLine($"pulling latest for {repoName}");
            var processInfo = new ProcessStartInfo("git", "pull");
            processInfo.WorkingDirectory = repoPath;
            this.StartProcess(processInfo);
        }

        public void Status(string repoName, string _)
        {
            var repoPath = Path.Combine(this.RootPath, repoName);
            var processInfo = new ProcessStartInfo("git", "status -bs");
            processInfo.WorkingDirectory = repoPath;
            this.Logger.WriteLine($"-------- {repoName} --------");
            this.StartProcess(processInfo);
        }

        public string[] GetGitRepoFolders()
        {
            var paths = Directory.GetDirectories(this.RootPath).Where(dir =>
                    Directory.GetDirectories(dir, ".git").Count() > 0
                );

            return paths.Select(p => p.Replace(this.RootPath, "").Trim('\\')).ToArray();
        }

        public virtual void StartProcess(ProcessStartInfo processStartInfo)
        {
            Process.Start(processStartInfo)?.WaitForExit();
        }
    }
}
