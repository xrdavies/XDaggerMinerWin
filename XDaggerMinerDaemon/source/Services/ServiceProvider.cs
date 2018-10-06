using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;
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

        /// <summary>
        /// ServiceType: 1: XDagger 2: Eth
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static ServiceProvider GetServiceProvider(MinerConfig.InstanceTypes serviceType)
        {
            switch(serviceType)
            {
                case MinerConfig.InstanceTypes.XDagger:
                    return new XDaggerServiceProvider();
                case MinerConfig.InstanceTypes.Eth:
                    return new EthServiceProvider();
                default:
                    return null;
            }
        }

        ///= @"XDaggerMinerService";
        //// protected string ServiceBinaryName = @"XDaggerMinerService.exe";
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetServiceName(int instanceId)
        {
            return (instanceId == 0) ? GetServiceNameBase() : string.Format("{0}_{1}", GetServiceNameBase(), instanceId);
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
        public ServiceInstance InstallService(int instanceId)
        {
            if (instanceId == 0)
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/instance=" + instanceId, ServiceBinaryFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { ServiceBinaryFullPath });
            }

            return AquaireInstance(instanceId);
        }

        public void UninstallService(int instanceId)
        {
            if (instanceId == 0)
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", "/instance=" + instanceId, ServiceBinaryFullPath });
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", ServiceBinaryFullPath });
            }
        }

        public ServiceInstance AquaireInstance(int instanceId)
        {
            ServiceInstance instance = new ServiceInstance(GetServiceName(instanceId), instanceId);
            instance.NamedPipeName = string.Format(GetNamedPipeNameTemplate(), instanceId);

            return instance;
        }

        public int DetectAvailableInstanceId()
        {
            int instanceId = 0;
            while (ServiceInstance.CheckServiceExist(GetServiceName(instanceId)))
            {
                instanceId++;
            }

            return instanceId;
        }
    }
}
