using ProfiseeDevUtils.Infrastructure;
using System.Diagnostics;
using System.ServiceProcess;

namespace ProfiseeDevUtils.Build
{
    public class Utils
    {
        public ILogger Logger { get; set; }

        public Utils(bool? quiet = false)
        {
             this.Logger = new Logger(quiet);
        }

        public string[] GetFilesByType(string root, string type)
        {
            return Directory.GetFiles(root, $"*.{type}", SearchOption.AllDirectories);
        }

        public string[] GetFoldersByName(string root, string dirName)
        {
            return Directory.GetDirectories(root, dirName, SearchOption.AllDirectories);
        }

        public string? GetFolderByFileName(string root, string fileName) => Directory.GetFiles(root, $"{fileName}.*", SearchOption.AllDirectories).FirstOrDefault();


        public List<string> GetDefaultSlns(string root)
        {
            var gits = Directory.GetDirectories(root);
            List<string> slns = new List<string>();
            foreach (var g in gits)
            {
               slns.AddRange(Directory.GetFiles(g, $"*.sln"));
            }

            return slns;
        }
    }
}
