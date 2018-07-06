using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerRuntimeCLI;
using XDaggerMinerDaemon.Commands.Outputs;
using XDaggerMiner.Common;

namespace XDaggerMinerDaemon.Commands
{
    public class ListDevicesCommand : ConsoleCommand
    {
        public override string GetShortName()
        {
            return "-l";
        }

        public override string GetLongName()
        {
            return "--List";
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            CommandInstance instance = new CommandInstance(this);

            return instance;
        }

        public override CommandResult Execute(string parameter)
        {
            // List all devices
            MinerManager minerManager = new MinerManager();
            ConsoleLogger logger = new ConsoleLogger();
            minerManager.SetLogger(logger);

            try
            {
                List<MinerDevice> deviceList = minerManager.GetAllMinerDevices();
                List<MinerDeviceOutput> deviceOutputList = new List<MinerDeviceOutput>();
                foreach (MinerDevice device in deviceList)
                {
                    MinerDeviceOutput outp = new MinerDeviceOutput(device);
                    deviceOutputList.Add(outp);
                }

                return CommandResult.CreateResult(deviceOutputList);
            }
            catch(Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }
    }
}
