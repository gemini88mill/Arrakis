namespace ProfiseeDevUtils.Infrastructure
{
    public interface ILogger
    {
        bool Quiet { get; }
        void Inform(string message);
        void Warn(string message);
        void Err(string message);
    }
}
