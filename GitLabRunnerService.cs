using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GitlabRunnerWrapper
{
    public partial class GitLabRunnerService : ServiceBase
    {
        private EventLog eventLog;

        public GitLabRunnerService()
        {
            this.ServiceName = "GitlabRunnerWrapper";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = false;

            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("GitLabRunnerWrapper"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "GitLabRunnerWrapper", "Application");
            }
            // configure the event log instance to use this source name
            eventLog.Source = "GitLabRunnerWrapper";
            eventLog.Log = "Application";

        }

        protected override void OnStart(string[] args)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                var appSettings = ConfigurationManager.AppSettings;
                var script = "New-Item -Path \"C:\\\" -Name \"GitLab-Runner\" -ItemType \"directory\";" +
                    "Invoke-WebRequest -Uri " + appSettings["gitlab_runner_bin"] + " -OutFile C:\\GitLab-Runner\\gitlab-runner.exe;" +
                    "C:\\GitLab-Runner\\gitlab-runner.exe register " +
                        "--non-interactive " +
                        "--url \"" + appSettings["gitlab_url"] + "\" " +
                        "--registration-token " + appSettings["gitlab_registration_token"] + " " +
                        "--tag-list " + appSettings["gitlab_tags"] + " " +
                        "--locked=\"false\" " +
                        "--executor shell;" + 
                "C:\\GitLab-Runner\\gitlab-runner.exe install;" +
                "C:\\GitLab-Runner\\gitlab-runner.exe start";

                PowerShellInstance.AddScript(script);
                //PowerShellInstance.AddScript(script);
                Collection <PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        eventLog.WriteEntry(outputItem.BaseObject.ToString());
                    }
                }
            }
            eventLog.WriteEntry("Service started");
        }

        protected override void OnStop()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                var script = "C:\\GitLab-Runner\\gitlab-runner.exe stop;" +
                    "C:\\GitLab-Runner\\gitlab-runner.exe uninstall;" +
                    "rmdir /s C:\\GitLab-Runner";
                PowerShellInstance.AddScript(script);

                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        eventLog.WriteEntry(outputItem.BaseObject.ToString());
                    }
                }
            }
            eventLog.WriteEntry("Service stoped");
        }

        internal void TestStartupAndStop()
        {
            this.OnStart(new String[] { });
            Console.ReadLine();
            this.OnStop();
        }
    }
}
