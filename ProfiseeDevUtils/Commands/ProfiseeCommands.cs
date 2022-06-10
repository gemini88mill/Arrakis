using ProfiseeDevUtils.Build;
using ProfiseeDevUtils.Init;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfiseeDevUtils.Commands
{
    public class ProfiseeCommands
    {

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
            new Option<bool?>( new[] { "-q", "--quiet" }, "Only Output is errors, if any" ),
        };

        public Command init = new Command("init", "Initiates a new Developer Instance")
        {

        };

        public Command git = new Command("git", "Perform Git operations on a project")
        {
            new Argument<string>("action", "type of action (status, push, pull, merge...)"),
            new Option<string>("repo", "repo to perform operation on"),
            new Option<string>("branch", "branch to perform git operation on"),
        };

        public ProfiseeCommands()
        {
            build.Handler = CommandHandler.Create<string?, string?, string?, string?, bool?, bool?, bool?, IConsole>(HandleBuild);
            config.Handler = CommandHandler.Create<bool?, IConsole>(HandleConfig);
            envVars.Handler = CommandHandler.Create<bool?, IConsole>(HandleEnvVars);
            init.Handler = CommandHandler.Create<IConsole>(HandleInit);
            git.Handler = CommandHandler.Create<string, string, string, IConsole>(HandleGit);
        }

        private void HandleGit(string action, string repo, string branch, IConsole console)
        {
            new Git().Act(action, repo, branch);
        }

        private void HandleInit(IConsole console)
        {
            console.WriteLine("not implemented guy!!!");
        }

        private void HandleConfig(bool? arg1, IConsole console)
        {
            console.WriteLine("This command is not implemented yet");
        }

        private void HandleEnvVars(bool? quiet, IConsole console)
        {
            console.WriteLine("Setting environment variables");
            new EnvironmentVariables(quiet).SetAll();
            console.WriteLine("Environment variables set!");
        }

        private void HandleBuild(string? name, string? git, string? data, string? config, bool? quiet, bool? log, bool? nuget, IConsole console)
        {
            var getgits = Directory.GetDirectories(@"C:\DevOps\Repos", ".git", SearchOption.AllDirectories);
            var repos = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Repo to build")
                    .AddChoices(getgits)
                );
        }
    }
}
