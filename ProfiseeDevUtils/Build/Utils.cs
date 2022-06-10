﻿using Cake.Common.Diagnostics;
using ProfiseeDevUtils.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProfiseeDevUtils.Build
{
    public class Utils
    {
        public ILogger Logger { get; set; } = new Logger();

        public void TurnOnService(string name)
        {
            ServiceController service = new ServiceController(name);
            if(service.Status == ServiceControllerStatus.Running)
            {
                this.Logger.WriteLine("Service is already started");
                return;
            }
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
            this.Logger.WriteLine("Service has successfully started");
        }

        public void TurnOffService(string name)
        {
            ServiceController service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                this.Logger.WriteLine("Service is already stopped");
                return;
            }
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
            this.Logger.WriteLine("Service has successfully stopped");
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
