using NSubstitute;
using NUnit.Framework;
using ProfiseeDevUtils;
using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;
using System.IO;

namespace ProfiseeDevUtilsTest
{
    internal class GitTests
    {
        private Git gitMock;
        private ILogger logger;
        private string gitUrl = $"https://profisee.visualstudio.com/Products/_git/";

        [SetUp]
        public void Setup()
        {
            this.logger = Substitute.For<ILogger>();
            this.gitMock = Substitute.For<Git>(false);
            this.gitMock.Logger = this.logger;
        }

        [Test]
        public void Git_Clone_Runs_git_clone_url()
        {
            var repoName = "testRepo";
            var url = $"{gitUrl}{repoName}";
            this.gitMock.Clone(repoName, string.Empty);

            this.gitMock.Received(1).StartProcess(Arg.Is <ProcessStartInfo>(p =>
                p.FileName == "git" &&
                p.Arguments == $"clone {url}"
                ));
            this.logger.Received(1).Inform(Arg.Any<string>());
            this.logger.Received(1).Inform($"cloning {url}");
        }

        [Test]
        public void Git_Clone_ExistingRepo_JustLogs()
        {
            var repoName = "BuildTools";
            var repoPath = Path.Combine(this.gitMock.RootPath, repoName);
            this.gitMock.Clone(repoName, string.Empty);

            this.gitMock.Received(0).StartProcess(Arg.Any<ProcessStartInfo>());
            this.logger.Received(1).Warn(Arg.Any<string>());
            this.logger.Received(1).Warn($"Repo {repoName} has already been created at {repoPath}");
        }

        [Test]
        public void Git_Pull_Runs_git_pull()
        {
            var repoName = "testRepo";
            this.gitMock.Pull(repoName, string.Empty);

            this.gitMock.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
               p.FileName == "git" &&
               p.Arguments == $"pull"
                ));
            this.logger.Received(1).Inform(Arg.Any<string>());
            this.logger.Received(1).Inform($"pulling latest for {repoName}");
        }

        [Test]
        public void Git_Status_Runs_git_status_bs()
        {
            var repoName = "testRepo";
            this.gitMock.Status(repoName, string.Empty);

            this.gitMock.Received(1).StartProcess(Arg.Is<ProcessStartInfo>(p =>
               p.FileName == "git" &&
               p.Arguments == $"status -bs"
                ));
            this.logger.Received(1).Inform(Arg.Any<string>());
            this.logger.Received(1).Inform($"-------- {repoName} --------");
        }

        [Test]
        public void Git_SetsLoggerQuietToTrue()
        {
            this.gitMock = Substitute.For<Git>(true);
            var logger = this.gitMock.Logger;

            Assert.IsTrue(logger.Quiet);
        }

        [Test]
        public void Git_SetsLoggerQuietToFalse()
        {
            this.gitMock = Substitute.For<Git>(false);
            var logger = this.gitMock.Logger;

            Assert.IsFalse(logger.Quiet);
        }
    }
}
