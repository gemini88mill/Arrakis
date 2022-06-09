// See https://aka.ms/new-console-template for more information
using Cake.Frosting;
using ProfiseeDevUtils.Build;
using ProfiseeDevUtils.Commands;
using System.CommandLine;
using static ProfiseeDevUtils.Build.BuildContext;

Console.WriteLine("Hello, World!");

Utils utils = new Utils();

var getUtils = utils.GetFilesByType(@"C:\DevOps\Repos", "sln");
var getBins = Directory.GetDirectories(@"C:\DevOps\Repos\platform", "bin", SearchOption.AllDirectories);

//foreach(var item in getUtils)
//{
//    Console.WriteLine(item);
//}

//return new CakeHost()
//            .UseContext<BuildContext>()
//            .UseLifetime<BuildLifetime>()
//            .Run(args);

var profCommands = new ProfiseeCommands();
var cmd = new RootCommand 
{
    profCommands.build
};

return cmd.Invoke(args);


