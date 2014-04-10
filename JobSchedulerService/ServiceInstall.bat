set path=%path%;C:\Windows\Microsoft.NET\Framework\v4.0.30319

installutil /i JobSchedulerService.exe

sc start JobSchedulerService

echo Service installed and started.