namespace ProfiseeDevUtils.Infrastructure
{
    public interface ILogger
    {
        void WriteLine(string message);
        void Write(string message);
    }
}
