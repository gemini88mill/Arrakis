using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProfiseeDevUtils.Build
{
    public class Utils
    {

        public void TurnOnService(string name)
        {
            ServiceController service = new ServiceController(name);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
        }

        public void TurnOffService(string name)
        {

        }

        public void GetListOfAvailableProjects(string root)
        {

        }


    }
}
