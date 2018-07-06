using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;

namespace XDaggerMinerDaemon.Utils
{
    public class ServiceUtil
    {
        public static readonly string ServiceName = @"XDaggerMinerService";
        public static readonly string ServiceBinaryName = @"XDaggerMinerService.exe";
        
        public static void InstallService(string serviceFullPath)
        {
            ManagedInstallerClass.InstallHelper(new string[] { serviceFullPath });
        }

        public static void UninstallService(string serviceFullPath)
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", serviceFullPath });
        }

        public static void StartService(string serviceName)
        {
            if (!CheckServiceExist(ServiceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(ServiceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public static void StopService(string serviceName)
        {
            if (!CheckServiceExist(ServiceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(ServiceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }

        public static bool CheckServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return (services.FirstOrDefault(s => s.ServiceName == serviceName) != null);
        }

        public static bool IsServiceRunning(string serviceName)
        {
            if (!CheckServiceExist(serviceName))
            {
                return false;
            }

            ServiceController[] services = ServiceController.GetServices();
            ServiceController service = services.FirstOrDefault(s => s.ServiceName == serviceName);

            return service.Status == ServiceControllerStatus.Running;
        }
    }
}
