using NSubstitute;
using NUnit.Framework;
using ProfiseeDevUtils.Infrastructure;
using ProfiseeDevUtils.Init;
using System.Diagnostics;

namespace ProfiseeDevUtilsTest
{
    internal class PreReqsTests
    {
        private PreReqs preReqsMock;
        private ILogger logger;

        [SetUp]
        public void Setup()
        {
            this.logger = Substitute.For<ILogger>();
            this.preReqsMock = Substitute.For<PreReqs>(false);
            this.preReqsMock.Logger = this.logger;
        }

        [Test]
        public void PreReqTests_Cheq_ReturnsTrue_dotnet6_0_dotnet3_1()
        {
            this.preReqsMock.StartProcess(new ProcessStartInfo()).ReturnsForAnyArgs(" 6.0.1, 6.0.1, 6.0.1, 3.1.18, 3.1.18, 3.1.18");
            var result = this.preReqsMock.Cheq();

            this.logger.Received(1).WriteLine("Found 3 versions of dotnet 3.1");
            this.logger.Received(1).WriteLine("Found 3 versions of dotnet 6.0");
            Assert.IsTrue(result);
        }

        [Test]
        public void PreReqTests_Cheq_ReturnsFalse_dotnet6_0_dotnet3_1_NotFound()
        {
            this.preReqsMock.StartProcess(new ProcessStartInfo()).ReturnsForAnyArgs("");
            var result = this.preReqsMock.Cheq();

            this.logger.Received(1).WriteLine("Found 0 versions of dotnet 3.1");
            this.logger.Received(1).WriteLine("Found 0 versions of dotnet 6.0");
            Assert.IsFalse(result);
        }

        [Test]
        public void PreReqTests_Cheq_ReturnsFalse_dotnet6_0_NotFound()
        {
            this.preReqsMock.StartProcess(new ProcessStartInfo()).ReturnsForAnyArgs(" 3.1.1, 3.1.1, 3.1.1,");
            var result = this.preReqsMock.Cheq();

            this.logger.Received(1).WriteLine("Found 3 versions of dotnet 3.1");
            this.logger.Received(1).WriteLine("Found 0 versions of dotnet 6.0");
            Assert.IsFalse(result);
        }

        [Test]
        public void PreReqTests_Cheq_Calls_donet_listRuntimes()
        {
            var result = this.preReqsMock.Cheq();

            this.preReqsMock.Received(2).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "dotnet" &&
                p.Arguments == "--list-runtimes"
                ));
        }

        [Test]
        public void PreReqTests_Cheq_Quiet_LogsLittle()
        {
            this.preReqsMock = Substitute.For<PreReqs>(true);
            this.preReqsMock.StartProcess(new ProcessStartInfo()).ReturnsForAnyArgs("");
            var result = this.preReqsMock.Cheq();

            this.logger.Received(0).WriteLine(Arg.Any<string>());
            Assert.IsFalse(result);
        }
    }
}
