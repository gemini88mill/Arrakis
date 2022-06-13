using Spectre.Console;

namespace ProfiseeDevUtils.Infrastructure
{
    public class Logger : ILogger
    {
        public bool Quiet { get; private set; }

        public Logger(bool? quiet)
        {
            this.Quiet = quiet ?? false;
        }

        public void Err(string message)
        {
            this.WriteLine($"[bold red]{message}[/]");
        }

        public void Inform(string message)
        {
            if (Quiet)
            {
                return;
            }

            this.WriteLine(message);
        }

        public void Warn(string message)
        {
            if (Quiet)
            {
                return;
            }

            this.WriteLine($"[bold yellow]{message}[/]");
        }

        public virtual void WriteLine(string message)
        {
            AnsiConsole.MarkupLine(message);
        }
    }
}
