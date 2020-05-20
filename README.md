# Gitlab Runner Wrapper
Gitlab Runner Wrapper is a Windows Service that download the gitlab runner binary, configure it and install it on start.

When you have a pool of windows VM that will be use as a pool of gitlab runner, the service will configure and register the runner on your gitlab instance when windows boot. 

## Deploy the service
Once the project is build, you will have 4 files:
- gitlab-runner-wrapper.exe
- gitlab-runner-wrapper.exe.config
- Microsoft.Management.Infrastructure.dll
- System.Management.Automation.dll

Edit the `gitlab-runner-wrapper.exe.config` file with your gitlab server url, the runner registration token and the tags you want to assign to your runner.

Upload the files on your server and install the service:
```powershell
New-Service -Name GitlabRunnerWrapper -BinaryPathName  C:\Users\localadmin\gitlab-runner-wrapper\gitlab-runnner-wrapper.exe -StartupType Automatic
Start-Service GitlabRunnerWrapper
```

That all, the service will create the directory `C:\Gitlab-Runner`, download the gitlab-runner.exe, configure it with the parameter in the `gitlab-runner-wrapper.exe.config` file and start the service.

You should be able to see the new agent on the gitlab admin now.