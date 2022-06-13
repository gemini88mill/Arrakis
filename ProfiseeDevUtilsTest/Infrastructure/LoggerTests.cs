using NSubstitute;
using NUnit.Framework;
using ProfiseeDevUtils.Infrastructure;

namespace ProfiseeDevUtilsTest.Infrastructure
{
    internal class LoggerTests
    {
        private Logger? logger;

        [SetUp]
        public void Setup()
        {
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Logger_Quiet(bool quiet)
        {
            this.logger = Substitute.For<Logger>(quiet);
            Assert.AreEqual(quiet, this.logger.Quiet);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Logger_Err(bool quiet)
        {
            var message = TestContext.CurrentContext.Random.GetString();
            this.logger = Substitute.For<Logger>(quiet);
            this.logger.Err(message);

            this.logger.Received(1).WriteLine($"[bold red]{message}[/]");
        }

        [TestCase(false, 1)]
        [TestCase(true, 0)]
        public void Logger_Warn(bool quiet, int count)
        {
            var message = TestContext.CurrentContext.Random.GetString();
            this.logger = Substitute.For<Logger>(quiet);
            this.logger.Warn(message);

            this.logger.Received(count).WriteLine($"[bold yellow]{message}[/]");
        }

        [TestCase(false, 1)]
        [TestCase(true, 0)]
        public void Logger_Inform(bool quiet, int count)
        {
            var message = TestContext.CurrentContext.Random.GetString();
            this.logger = Substitute.For<Logger>(quiet);
            this.logger.Inform(message);

            this.logger.Received(count).WriteLine($"{message}");
        }
    }
}
