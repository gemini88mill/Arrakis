using NSubstitute;
using NUnit.Framework;
using ProfiseeDevUtils.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfiseeDevUtilsTest.Infrastructure
{
    internal class IISTests
    {
        private ILogger logger;

        [SetUp]
        public void Setup()
        {
            this.logger = Substitute.For<ILogger>();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IIS_Start(bool quiet)
        {
            var iis = Substitute.For<IIS>(quiet);
            iis.Logger = this.logger;

            iis.Start();

            iis.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "iisreset" &&
                p.Arguments == "/start"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IIS_Stop(bool quiet)
        {
            var iis = Substitute.For<IIS>(quiet);
            iis.Logger = this.logger;

            iis.Stop();

            iis.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
                p.FileName == "iisreset" &&
                p.Arguments == "/stop"));
        }



        [TestCase(true)]
        [TestCase(false)]
        public void IIS_Logger(bool quiet)
        {
            var iis = Substitute.For<IIS>(quiet);
            
            Assert.AreEqual(quiet, iis.Logger.Quiet);
        }
    }
}
