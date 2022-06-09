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

namespace ProfiseeDevUtils.Build
{
    public class BuildContext : FrostingContext
    {
        public bool Delay { get; set; }
        public string MsBuildConfiguration { get; set; }

        public BuildContext(ICakeContext context)
            : base(context)
        {
            MsBuildConfiguration = context.Argument("configuration", "Release");
        }

        [TaskName("Clean")]
        public sealed class CleanTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                context.CleanDirectory($"../src/Example/bin/{context.MsBuildConfiguration}");
            }
        }

        [TaskName("Build")]
        [IsDependentOn(typeof(CleanTask))]
        public sealed class BuildTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                context.DotNetBuild("../src/Example.sln", new DotNetBuildSettings
                {
                    Configuration = context.MsBuildConfiguration,
                });
            }
        }

        [TaskName("Test")]
        [IsDependentOn(typeof(BuildTask))]
        public sealed class TestTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                context.DotNetTest("../src/Example.sln", new DotNetTestSettings
                {
                    Configuration = context.MsBuildConfiguration,
                    NoBuild = true,
                });
            }
        }

        [TaskName("Default")]
        [IsDependentOn(typeof(TestTask))]
        public class DefaultTask : FrostingTask
        {
        }
    }
}
