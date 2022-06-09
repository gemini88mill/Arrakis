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

        public string rootPath { get; set; }

        public BuildContext(ICakeContext context)
            : base(context)
        {
            MsBuildConfiguration = context.Argument("configuration", "Debug");
            rootPath = context.Argument("path", @"C:\DevOps\Repos\rest-api\");
        }

        //[TaskName("Clean")]
        //public sealed class CleanTask : FrostingTask<BuildContext>
        //{
        //    public override void Run(BuildContext context)
        //    {
        //        var bins = new Utils().GetFoldersByName(context.rootPath, "bin");

        //        context.CleanDirectory(Path.Combine(context.rootPath, context.MsBuildConfiguration));
        //    }
        //}

        [TaskName("Build")]
        //[IsDependentOn(typeof(CleanTask))]
        public sealed class BuildTask : FrostingTask<BuildContext>
        {
            public override void Run(BuildContext context)
            {
                context.DotNetBuild(context.rootPath + @"\Gateway.Api.sln", new DotNetBuildSettings
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
                context.DotNetTest(context.rootPath + @"\Gateway.Api.sln", new DotNetTestSettings
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

        public class BuildLifetime : FrostingLifetime<BuildContext>
        {
            public override void Setup(BuildContext context)
            {
                //new Utils().TurnOffService("W3SVC");
                //new Utils().TurnOnService("W3SVC");
                new Utils().TurnOffService("Profisee 22.2.0 (Profisee)");
            }

            public override void Teardown(BuildContext context, ITeardownContext info)
            {
                new Utils().TurnOnService("Profisee 22.2.0 (Profisee)");

            }
        }
    }
}
