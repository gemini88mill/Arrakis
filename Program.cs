// See https://aka.ms/new-console-template for more information
using ProfiseeDevUtils.Build;

Console.WriteLine("Hello, World!");

Utils utils = new Utils();

var getUtils = utils.GetListOfAvailableProjects(@"C:\DevOps\Repos", "sln");

foreach(var item in getUtils)
{
    Console.WriteLine(item);
}
