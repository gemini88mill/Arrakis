using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.Diagnostics;
using Cake.Git;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.DotNetCore;

namespace ProfiseeDevUtils.Build
{
    public class BuildContext : FrostingContext
    {
        public bool Delay { get; set; }
        public string MsBuildConfiguration { get; set; }

        public string rootPath { get; set; }
        public string solutionFullPath { get; set; }

        public Verbosity LogLevel { get; set; }
        public DotNetVerbosity DnVerbosity { get; set; }

        public FileInfo fileInfo { get; set; }

        public Utils utils = new Utils();

        public BuildContext(ICakeContext context)
            : base(context)
        {
            solutionFullPath = context.Argument("slnPath", @"C:\DevOps\Repos\rest-api\Gateway.Api.sln");
            fileInfo = new FileInfo(solutionFullPath);

            rootPath = fileInfo.DirectoryName;

            MsBuildConfiguration = context.Argument("configuration", "Debug");
            LogLevel = context.Argument("LogLevel", Verbosity.Normal);
            DnVerbosity = context.Argument("LogLevel", DotNetVerbosity.Normal);
        }

        [TaskName("Clean")]
        public sealed class CleanTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                if (context.LogLevel < Verbosity.Normal)
                {
                    context.QuietVerbosity();
                }
                //watch out of bin on node modules...
                context.CleanDirectories(context.rootPath + "/**/bin");

                
            }
        }

        [TaskName("Build")]
        [IsDependentOn(typeof(CleanTask))]
        public sealed class BuildTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                if(context.fileInfo.Name == "ProfiseePlatform")
                {
                    context.MSBuild(context.fileInfo.FullName, new MSBuildSettings
                    {
                        Verbosity = context.LogLevel,
                        Configuration = "Debug"
                    });
                }
                else
                {
                    context.DotNetBuild(context.solutionFullPath, new DotNetBuildSettings
                    {
                        Configuration = context.MsBuildConfiguration,
                        Verbosity = context.DnVerbosity
                    });
                }
            }
        }

        [TaskName("Test")]
        [IsDependentOn(typeof(BuildTask))]
        public sealed class TestTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                var slnFullPath = Directory.GetFiles(context.rootPath, "sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

                context.DotNetTest(context.solutionFullPath, new DotNetTestSettings
                {
                    Configuration = context.MsBuildConfiguration,
                    NoBuild = true,
                    Verbosity = context.DnVerbosity
                });
            }
        }

        [TaskName("Publish")]
        [IsDependentOn(typeof(BuildTask))]
        public sealed class PublishTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                context.DotNetPublish(context.fileInfo.FullName);
            }
        }

        [TaskName("GitPull")]
        public sealed class GitTask : FrostingTask<BuildContext>
        {
            public override bool ShouldRun(BuildContext context)
            {
                return false;
            }

            public override void Run(BuildContext context)
            {
                context.GitPull(context.rootPath, context.GitConfigGet<string>(context.rootPath, "user.name"), context.GitConfigGet<string>(context.rootPath, "user.email"));
            }
        }


        [TaskName("Default")]
        [IsDependentOn(typeof(TestTask))]
        public class DefaultTask : FrostingTask
        {
        }

        public class BuildLifetime : FrostingLifetime<BuildContext>
        {
            public override void Setup(BuildContext context)
            {
                context.DiagnosticVerbosity();
                new Utils().TurnOffService("Profisee 22.2.0 (Profisee)");
                new Utils().TurnOffService("W3SVC");
            }

            public override void Teardown(BuildContext context, ITeardownContext info)
            {
                new Utils().TurnOnService("Profisee 22.2.0 (Profisee)");

            }
        }
    }
}
