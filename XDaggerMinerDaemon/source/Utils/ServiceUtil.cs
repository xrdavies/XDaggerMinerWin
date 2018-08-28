using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common.Contracts;

namespace XDaggerMinerDaemon.Utils
{
    public class ServiceUtil
    {
        public static readonly string ServiceNameBase = @"XDaggerMinerService";
        public static readonly string ServiceBinaryName = @"XDaggerMinerService.exe";
        
        public static string GetServiceName(string instanceId)
        {
            return (string.IsNullOrEmpty(instanceId) ? ServiceNameBase : string.Format("{0}_{1}", ServiceNameBase, instanceId));
        }

        public static void InstallService(string serviceFullPath, string instanceId = "")
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/instance="+instanceId, serviceFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { serviceFullPath });
            }
        }

        public static void UninstallService(string serviceFullPath, string instanceId = "")
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", "/instance=" + instanceId, serviceFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", serviceFullPath });
            }
        }

        public static void StartService(string instanceId = "")
        {
            string serviceName = GetServiceName(instanceId);
            if (!CheckServiceExist(serviceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public static void StopService(string instanceId = "")
        {
            string serviceName = GetServiceName(instanceId);
            if (!CheckServiceExist(serviceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(serviceName);
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

        public static int? DetectAvailableInstanceId()
        {
            if (!CheckServiceExist(ServiceNameBase))
            {
                return null;
            }

            int instanceNumber = 1;

            while(CheckServiceExist(GetServiceName(instanceNumber.ToString())))
            {
                instanceNumber++;
            }

            return instanceNumber;
        }
    }
}
