using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon.Commands
{
    public class ConfigureCommand : ConsoleCommand
    {
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
            ConfigureParameter configure = null;

            try
            {
                configure = JsonConvert.DeserializeObject<ConfigureParameter>(parameter);
            }
            catch(FormatException)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "The format of the Configuration content is not valid Json.");
            }

            MinerConfig config = MinerConfig.ReadFromFile();

            if (!string.IsNullOrEmpty(configure.DeviceId))
            {
                MinerManager minerManager = new MinerManager();
                ConsoleLogger logger = new ConsoleLogger();
                minerManager.SetLogger(logger);

                List<MinerDevice> deviceList = minerManager.GetAllMinerDevices();
                bool deviceFound = false;
                foreach(MinerDevice device in deviceList)
                {
                    if (device.IsMatchId(configure.DeviceId))
                    {
                        config.Device = new MinerConfigDevice(device.GetDeviceId(), device.GetDisplayName(), device.GetDeviceVersion(), device.GetDriverVersion());
                        deviceFound = true;
                        break;
                    }
                }
                
                if (!deviceFound)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_DEVICE_NOT_FOUND, string.Format("Did not find the device matches DeviceId=[{0}]", configure.DeviceId));
                }
            }

            if (!string.IsNullOrEmpty(configure.Wallet))
            {
                // TODO: Should validate the Wallet address first
                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_FORMET_ERROR, string.Format("Wallet format is not correct. Wallet=[{0}]", configure.Wallet));
                }

                if (false)
                {
                    throw new TargetExecutionException(DaemonErrorCode.CONFIG_WALLET_NOT_FOUND, string.Format("Wallet cannot be found. Wallet=[{0}]", configure.Wallet));
                }

                config.WalletAddress = configure.Wallet;
            }

            try
            {
                config.SaveToFile();

                return CommandResult.OKResult();
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex.Message);
            }
        }
    }

    public class ConfigureParameter
    {
        public string DeviceId;

        /// <summary>
        /// The Wallet address of the customer
        /// </summary>
        public string Wallet;
    }

}
