using Cake.Frosting;
using ProfiseeDevUtils.Build;
using ProfiseeDevUtils.Infrastructure;
using ProfiseeDevUtils.Init;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using static ProfiseeDevUtils.Build.BuildContext;

namespace ProfiseeDevUtils.Commands
{
    public class ProfiseeCommands
    {
        public ILogger Logger { get; set; } = new Logger(false);

        public Command build = new Command("build", "builds the various projects of the Profisee Platform")
        {
            new Option<string?>(new[] {"-n", "--name"}, "Name of the project"),
            new Option<string?>(new[] {"-s", "--select"}, "Select the name of Project"),
            new Option<string?>(new[] {"-g", "--git"}, "get latest during build"),
            new Option<string?>(new[] {"-d", "--data"}, "populate instance with default data (run apollo)"),
            new Option<string?>(new[] {"-c", "--config"}, "after build config with default config settings"),
            new Option<bool?>(new[] {"-q", "--quiet"}, "Only display if errors are present"),
            new Option<bool?>(new[] {"-l", "--log"}, "output to log"),
            new Option<bool?>(new[] {"-nu", "--nuget" }, "Restore Nuget packages along with build")
        };

        public Command config = new Command("config", "configures a new instance of the Profisee platform on your machine :)")
        {
            new Option<bool?>(new[] {"-d", "--data"}, "Create a new instance with Data from Apollo test suite")
        };

        public Command envVars = new Command("envVars", "sets the environment variables for profisee")
        {
            new Option<bool?>( new[] { "-q", "--quiet" }, "Only output is errors, if any" ),
        };

        public Command init = new Command("init", "Initiates a new Developer Instance")
        {
            new Option<bool?>( new[] { "-q", "--quiet" }, "Only output minimal info" ),
        };

        public Command git = new Command("git", "Perform Git operations on a project")
        {
            new Argument<string>("action", "type of action (clone, status, push, pull, merge...)"),
            new Option<string>(new[] { "-r", "--repo" }, "repo to perform operation on"),
            new Option<string>(new[] { "-b", "--branch" }, "branch to perform git operation on"),
        };

        public Command iis = new Command("iis", "Perform IIS operations")
        {
            new Argument<string>("action", "type of action (start, stop, reset")
        };

        public Command profisee = new Command("profisee", "Perform operations on the Profisee service")
        {
            new Argument<string>("action", "type of action (start, stop"),
            new Option<bool?>( new[] { "-q", "--quiet" }, "Only output minimal info" ),
        };

        public Command dropDb = new Command("dropDb", "Drop Profisee database(s)")
        {
            new Option<string>(new[] { "-s", "--sql" }, "sql server to perform operation on"),
            new Option<string>(new[] { "-d", "--database" }, "sql database to drop"),
            new Option<bool?>( new[] { "-q", "--quiet" }, "Only output minimal info" ),
        };

        public ProfiseeCommands()
        {
            build.Handler = CommandHandler.Create<string?, /*string?, string?, string?,*/ bool?, bool?, bool?, IConsole>(HandleBuildAsync);
            config.Handler = CommandHandler.Create<bool?, IConsole>(HandleConfig);
            envVars.Handler = CommandHandler.Create<bool?, IConsole>(HandleEnvVars);
            init.Handler = CommandHandler.Create<bool?, IConsole>(HandleInit);
            git.Handler = CommandHandler.Create<string, string, string, IConsole>(HandleGit);
            iis.Handler = CommandHandler.Create<string, IConsole>(HandleIIS);
            profisee.Handler = CommandHandler.Create<string, bool?>(Profisee.Service.Act);
            dropDb.Handler = CommandHandler.Create<string?, string?, bool?>(Database.DropDB.Act);
        }

        private void HandleIIS(string action, IConsole console)
        {
            var sanitizedAction = action.ToLower();
            var iis = new IIS();
            var actions = new Dictionary<string, Action>
            {
                { "start", iis.Start },
                { "stop", iis.Stop },
                { "reset", iis.Reset },
            };

            if (!actions.ContainsKey(sanitizedAction))
            {
                this.Logger.Err($"Action {action} not found in available actions. Please use one of the following:");
                foreach(var a in actions.Keys)
                {
                    this.Logger.Err(a);
                }
                return;
            }

            actions[sanitizedAction]();
        }

        private void HandleGit(string action, string repo, string branch, IConsole console)
        {
            var git = new Git();
            if (string.IsNullOrEmpty(repo) && action.ToLower() != "clone")
            {
                var repos = git.GetGitRepoFolders();

                repos = repos.Prepend("All").ToArray();
                repo = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Select Repo to run 'git {action}' on")
                        .AddChoices(repos)
                    );
                if (repo.ToLower() == "all")
                {
                    repo = string.Empty;
                }
            }

            git.Act(action, repo, branch);
        }

        private void HandleInit(bool? quiet, IConsole console)
        {
            console.WriteLine("Checking prereqs...");
            if (!new PreReqs(quiet).Cheq())
            {
                AnsiConsole.MarkupLine("[bold red]You don't have the required prereqs. Install the ones reported above first and run again[/]");
                return;
            }

            console.WriteLine("Initializing new dev box...");
            var envVariables = new EnvironmentVariables(quiet);
            envVariables.CreateCustomVarsFile();

            envVariables.SetAllAsync().Wait();

            new Git().Act("clone", string.Empty, string.Empty);

            console.WriteLine("Finished setting up new dev box");
            AnsiConsole.MarkupLine("You [bold yellow]should[/] close this window and reopen to get the latest variables");
            console.WriteLine("Happy coding!!");
        }

        private void HandleConfig(bool? quiet, IConsole console)
        {
            console.WriteLine("This command is not implemented yet");
        }

        private void HandleEnvVars(bool? quiet, IConsole console)
        {
            console.WriteLine("Setting environment variables");
            new EnvironmentVariables(quiet).SetAllAsync().Wait();
            console.WriteLine("Environment variables set!");
            AnsiConsole.Markup("You [bold yellow]should[/] close this window and reopen to get the latest variables");
        }

        private async Task<int> HandleBuildAsync(string? name, /*string? git, string? data, string? config,*/ bool? quiet, bool? log, bool? nuget, IConsole console)
        {
            Utils utils = new Utils();
            var envVars = new EnvironmentVariables(quiet);
            var root = envVars.gitRepos;
            var slns = new List<string>() { "All Repos" };
            string repo = "";

            if (string.IsNullOrEmpty(name))
            {
                slns.AddRange(utils.GetDefaultSlns(root));

                repo = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Repo to build")
                    .AddChoices(slns)
                );
            }
            else
            {
                await AnsiConsole.Status()
                    .StartAsync("Fetching Project", ctx => {
                        repo = utils.GetFolderByFileName(root, name) ?? string.Empty;
                        return Task.CompletedTask;
                    });
            }

            return new CakeHost()
                .UseContext<BuildContext>()
                .UseLifetime<BuildLifetime>()
                .Run(new[]
                {
                    $"--rootPath={new DirectoryInfo(repo).Name}",
                    $"--logLevel={((quiet ?? false) ? 0 : 2)}",
                    $"--slnPath={repo}",
                });
        }
    }
}
