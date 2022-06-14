using NSubstitute;
using NUnit.Framework;
using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;

namespace ProfiseeDevUtilsTest.Infrastructure
{
    internal class IISTests
    {
        [Test]
        public void IIS_Start()
        {
            var iis = Substitute.For<IIS>();

            iis.Start();

            iis.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "iisreset" &&
                p.Arguments == "/start"));
        }

        [Test]
        public void IIS_Stop()
        {
            var iis = Substitute.For<IIS>();

            iis.Stop();

            iis.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "iisreset" &&
                p.Arguments == "/stop"));
        }

        [Test]
        public void IIS_Reset()
        {
            var iis = Substitute.For<IIS>();

            iis.Reset();

            iis.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "iisreset" &&
                p.Arguments == string.Empty));
        }
    }
}
