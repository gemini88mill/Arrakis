// See https://aka.ms/new-console-template for more information
using Cake.Frosting;
using ProfiseeDevUtils.Build;
using ProfiseeDevUtils.Commands;
using System.CommandLine;
using static ProfiseeDevUtils.Build.BuildContext;

Console.WriteLine("Hello, World!");

Utils utils = new Utils();
string root = @"C:\DevOps\Repos";

//var getProj = utils.GetFilesByType(@"C:\DevOps\Repos", "csproj");
//var getSln = utils.GetFilesByType(@"C:\DevOps\Repos", "sln");
//var getBins = Directory.GetDirectories(@"C:\DevOps\Repos\platform", "bin", SearchOption.AllDirectories);
//var getgits = Directory.GetDirectories(@"C:\DevOps\Repos", ".git", SearchOption.AllDirectories);

//foreach (var item in getSln.Concat(getProj))
//{
//    Console.WriteLine(item);
//}

//run cake
//return new CakeHost()
//            .UseContext<BuildContext>()
//            .UseLifetime<BuildLifetime>()
//            .Run(args);


//This is for the build lib
var profCommands = new ProfiseeCommands();
var cmd = new RootCommand
{
    profCommands.build,
    profCommands.config,
    profCommands.envVars,
    profCommands.init,
    profCommands.git
};

return cmd.Invoke(args);

//Console.WriteLine(utils.GetFolderByFileName(root, "Gateway.Api.sln"));


