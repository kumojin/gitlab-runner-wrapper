using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GitlabRunnerWrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*if (Environment.UserInteractive)
            {
                GitLabRunnerService service1 = new GitLabRunnerService();
                service1.TestStartupAndStop();
            } 
            else
            {*/
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new GitLabRunnerService()
                };
                ServiceBase.Run(ServicesToRun);
            //}
        }
    }
}
