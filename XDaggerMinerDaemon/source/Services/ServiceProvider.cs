using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common.Contracts;

namespace XDaggerMinerDaemon.Services
{
    public class ServiceProvider
    {
        protected virtual string GetServiceNameBase()
        {
            return string.Empty;
        }

        protected virtual string GetServiceBinaryName()
        {
            return string.Empty;
        }

        protected virtual string GetNamedPipeNameTemplate()
        {
            return string.Empty;
        }

        ///= @"XDaggerMinerService";
        //// protected string ServiceBinaryName = @"XDaggerMinerService.exe";
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetServiceName(string instanceId)
        {
            return (string.IsNullOrEmpty(instanceId) ? GetServiceNameBase() : string.Format("{0}_{1}", GetServiceNameBase(), instanceId));
        }

        /// <summary>
        /// 
        /// </summary>
        private string ServiceBinaryFullPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GetServiceBinaryName());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ServiceInstance InstallService(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/instance=" + instanceId, ServiceBinaryFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { ServiceBinaryFullPath });
            }

            return AquaireInstance(instanceId);
        }

        public void UninstallService(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", "/instance=" + instanceId, ServiceBinaryFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", ServiceBinaryFullPath });
            }
        }

        public ServiceInstance AquaireInstance(string instanceId)
        {
            ServiceInstance instance = new ServiceInstance(GetServiceName(instanceId), instanceId);
            instance.NamedPipeName = string.Format(GetNamedPipeNameTemplate(), instanceId);

            return instance;
        }

        public int? DetectAvailableInstanceId()
        {
            if (!ServiceInstance.CheckServiceExist(GetServiceNameBase()))
            {
                return null;
            }

            int instanceNumber = 1;

            while (ServiceInstance.CheckServiceExist(GetServiceName(instanceNumber.ToString())))
            {
                instanceNumber++;
            }

            return instanceNumber;
        }

    }
}
