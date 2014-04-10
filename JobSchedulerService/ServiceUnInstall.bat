set path=%path%;C:\Windows\Microsoft.NET\Framework\v4.0.30319

sc stop JobSchedulerService

installutil /u JobSchedulerService.exe

echo Service uninstalled from the system.