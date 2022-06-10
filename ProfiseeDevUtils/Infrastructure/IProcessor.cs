using System.Diagnostics;

namespace ProfiseeDevUtils.Infrastructure
{
    public interface IProcessor
    {
        IProcessor Start(ProcessStartInfo processStartInfo);
        void WaitForExit();
        StreamReader? StandardOutput { get; }
    }
}
