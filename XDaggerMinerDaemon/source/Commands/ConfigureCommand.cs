using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDaggerMiner.Common;
using XDaggerMiner.Common.Contracts;
using XDaggerMiner.Common.Contracts.CommandOutputs;
using XDaggerMiner.Common.Utils;
using XDaggerMinerDaemon.Services;
using XDaggerMinerDaemon.Utils;
using XDaggerMinerRuntimeCLI;
using static XDaggerMiner.Common.MinerConfig;

namespace XDaggerMinerDaemon.Commands
{
    public class ConfigureCommand : ConsoleCommand
    {
        private bool isInstanceUpdated = false;

        public override string GetShortName()
        {
            return "-c";
        }

        public override string GetLongName()
        {
            return "--Configure";
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            if (arguments.Length <= nextIndex)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "Argument Not Correct for Configure Command.");
            }

            CommandInstance instance = new CommandInstance(this);
            instance.Parameter = arguments[nextIndex++];

            return instance;
        }

        public override CommandResult Execute(string parameter)
        {
            // Update the configuration
            ConfigureParameter configParameters = null;
            isInstanceUpdated = false;

            try
            {
                configParameters = JsonConvert.DeserializeObject<ConfigureParameter>(parameter);
            }
            catch(FormatException)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "The format of the Configuration content is not valid Json.");
            }

            MinerConfig config = MinerConfig.ReadFromFile();

            if (!string.IsNullOrEmpty(configParameters.DeviceId) || !string.IsNullOrEmpty(configParameters.DeviceName))
            {
                MinerManager minerManager = new MinerManager();

                List<MinerDevice> deviceList = minerManager.GetAllMinerDevices();
                bool deviceFound = false;
                foreach(MinerDevice device in deviceList)
                {
                    if (device.IsMatchId(configParameters.DeviceId) || string.Equals(device.GetDisplayName(), configParameters.DeviceName))
                    {
                        config.Device = new MinerConfigDevice(device.GetDeviceId(), device.GetDisplayName(), device.GetDeviceVersion(), device.GetDriverVersion());
                        deviceFound = true;
                        break;
                    }
                }
                
                if (!deviceFound)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_DEVICE_NOT_FOUND, string.Format("Did not find the device matches DeviceId=[{0}] or DeviceName=[{1}]", 
                        configParameters.DeviceId,
                        configParameters.DeviceName));
                }
            }

            if ((!string.IsNullOrEmpty(configParameters.XDaggerWallet) || !string.IsNullOrEmpty(configParameters.XDaggerPoolAddress))
                && !string.IsNullOrEmpty(configParameters.EthPoolAddress))
            {
                throw new TargetExecutionException(DaemonErrorCode.CONFIG_ONLY_ONE_INSTANCE_TYPE_ALLOWED, "Only one type of miner instance is allowed.");
            }

            if (configParameters.InstanceId.HasValue && configParameters.AutoDecideInstanceId)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "Cannot specify InstanceId while AutoDecideInstanceId is used.");
            }
            
            InstanceTypes? proposedInstanceType = null;

            if (!string.IsNullOrEmpty(configParameters.XDaggerWallet))
            {
                string wallet = configParameters.XDaggerWallet.Trim();

                // TODO: Should validate the Wallet address first
                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_FORMET_ERROR, string.Format("Wallet format is not correct. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_NOT_FOUND, string.Format("Wallet cannot be found. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (config.XDaggerMiner == null)
                {
                    config.XDaggerMiner = new XDaggerMinerConfig();
                }

                proposedInstanceType = MinerConfig.InstanceTypes.XDagger;
                config.XDaggerMiner.WalletAddress = wallet;
            }

            if (!string.IsNullOrEmpty(configParameters.XDaggerPoolAddress))
            {
                string poolAddress = configParameters.XDaggerPoolAddress.Trim();

                // TODO: Should validate the Wallet address first
                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_FORMET_ERROR, string.Format("Wallet format is not correct. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_NOT_FOUND, string.Format("Wallet cannot be found. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (config.XDaggerMiner == null)
                {
                    config.XDaggerMiner = new XDaggerMinerConfig();
                }

                proposedInstanceType = MinerConfig.InstanceTypes.XDagger;
                config.XDaggerMiner.PoolAddress = poolAddress;
            }

            if (!string.IsNullOrEmpty(configParameters.EthPoolAddress))
            {
                string ethPoolAddress = configParameters.EthPoolAddress.Trim();

                // TODO: Should validate the Wallet address first
                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_FORMET_ERROR, string.Format("Wallet format is not correct. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_NOT_FOUND, string.Format("Wallet cannot be found. Wallet=[{0}]", configParameters.XDaggerWallet));
                }

                if (config.EthMiner == null)
                {
                    config.EthMiner = new EthMinerConfig();
                }

                proposedInstanceType = MinerConfig.InstanceTypes.Eth;
                config.EthMiner.PoolAddress = ethPoolAddress;
            }

            ServiceProvider currentServiceProvider = ServiceProvider.GetServiceProvider(config.InstanceType);
            ServiceInstance currentServiceInstance = currentServiceProvider?.AquaireInstance(config.InstanceId);

            // Check the change of instance Type, if the service is already installed, put the new instance type into updated_instance_type, and detect new updated_instance_id
            if (currentServiceInstance != null && currentServiceInstance.IsServiceExist())
            {
                // Put the type into updated_instance_type
                if(proposedInstanceType == null)
                {
                    if (config.UpdatedInstanceType != null && configParameters.InstanceId != null)
                    {
                        config.UpdatedInstanceId = configParameters.InstanceId;
                    }
                }
                else if (proposedInstanceType != config.InstanceType)
                {
                    isInstanceUpdated = true;
                    config.UpdatedInstanceType = proposedInstanceType;
                    config.UpdatedInstanceId = DecideInstanceId(configParameters, proposedInstanceType.Value, true);
                }
            }
            else if (proposedInstanceType != null)
            {
                if (configParameters.AutoDecideInstanceId || (config.InstanceType != InstanceTypes.Unset && proposedInstanceType != config.InstanceType))
                {
                    isInstanceUpdated = true;
                }

                config.InstanceType = proposedInstanceType.Value;
                config.InstanceId = DecideInstanceId(configParameters, proposedInstanceType.Value, false) ?? config.InstanceId;
                
                // Clear the updated ones
                config.UpdatedInstanceId = null;
                config.UpdatedInstanceType = null;
            }

            // Save all of the changes into config file
            int retryTimes = 0;
            while (true)
            {
                try
                {
                    config.SaveToFile();

                    if (isInstanceUpdated)
                    {
                        return CommandResult.CreateResult(ConfigureOutput.Create(config.UpdatedInstanceId ?? config.InstanceId));
                    }
                    else
                    {
                        return CommandResult.OKResult();
                    }
                }
                catch (Exception ex)
                {
                    if (retryTimes++ < IntermediateFailureRetryTimes)
                    {
                        Thread.Sleep(IntermediateFailureRetryPeriod);
                        continue;
                    }

                    throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex.Message);
                }
            }
        }

        private int? DecideInstanceId(ConfigureParameter parameter, InstanceTypes instanceType, bool force)
        {
            // Decide the instanceId
            if (parameter.InstanceId.HasValue)
            {
                return parameter.InstanceId.Value;
            }

            if (parameter.AutoDecideInstanceId || force)
            {
                ServiceProvider serviceProvider = ServiceProvider.GetServiceProvider(instanceType);
                if (serviceProvider == null)
                {
                    return null;
                }
                else
                {
                    return serviceProvider.DetectAvailableInstanceId();
                }
            }

            return null;
        }
    }

    public class ConfigureParameter
    {
        public int? InstanceId;

        public string DeviceId;

        public string DeviceName;

        /// <summary>
        /// The XDagger Wallet address of the customer
        /// </summary>
        public string XDaggerWallet;

        /// <summary>
        /// The Eth Miner Pool address
        /// </summary>
        public string EthPoolAddress;

        public string XDaggerPoolAddress;

        public bool AutoDecideInstanceId = false;
    }
}
