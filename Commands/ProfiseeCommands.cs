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
            new Option<string?>(new[] {"-g", "--git"}, "get latest during build"),
            new Option<string?>(new[] {"-d", "--data"}, "populate instance with default data (run apollo)"),
            new Option<string?>(new[] {"-c", "--config"}, "after build config with default config settings"),
        };

        public ProfiseeCommands()
        {
            

            build.Handler = CommandHandler.Create<string?, string?, string?, string?, IConsole>(HandleBuild);
        }

        private void HandleBuild(string? name, string? git, string? data, string? config, IConsole console)
        {
            console.WriteLine("This command is not implemented yet");
        }
    }
}
