using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common.Contracts;

namespace XDaggerMinerDaemon.Services
{
    public class ServiceInstance
    {
        private static readonly int SERVICE_OPERATION_WAIT_TIME = 10000;

        public string ServiceName
        {
            get; private set;
        }

        public string InstanceId
        {
            get; private set;
        }

        public string NamedPipeName
        {
            get; set;
        }

        public ServiceInstance(string serviceName, string instanceId)
        {
            this.ServiceName = serviceName;
            this.InstanceId = instanceId;
        }

        public void Start()
        {
            if (!CheckServiceExist(ServiceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(ServiceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(SERVICE_OPERATION_WAIT_TIME);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public void Stop()
        {
            if (!CheckServiceExist(ServiceName))
            {
                throw new TargetExecutionException(DaemonErrorCode.SERVICE_NOT_INSTALLED, "Service not installed");
            }

            ServiceController service = new ServiceController(ServiceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(SERVICE_OPERATION_WAIT_TIME);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
        
        public bool IsServiceRunning()
        {
            if (!IsServiceExist())
            {
                return false;
            }

            ServiceController[] services = ServiceController.GetServices();
            ServiceController service = services.FirstOrDefault(s => s.ServiceName == ServiceName);

            return service.Status == ServiceControllerStatus.Running;
        }

        public bool IsServiceExist()
        {
            return CheckServiceExist(ServiceName);
        }

        public static bool CheckServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return (services.FirstOrDefault(s => s.ServiceName == serviceName) != null);
        }

    }
}
