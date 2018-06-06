using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon
{
    public class MainConsole
    {
        private MinerConfig minerConfig = null;

        private MinerManager minerManager = null;

        private LoggerBase logger = null;

        private string[] rawArguments;


        public MainConsole(string[] args)
        {
            rawArguments = args;

            minerManager = new MinerManager();
            logger = new ConsoleLogger();
            minerManager.SetLogger(logger);

            try
            {
                minerConfig = MinerConfig.ReadFromFile();

            }
            catch(Exception ex)
            {
                logger.WriteLog(3, 0, "Read config file failed: " + ex.ToString());
            }

            
        }

        public void Execute()
        {
            if (minerConfig == null || minerManager == null)
            {
                logger.WriteLog(3, 0, "Cannot Execute due to previous initialization failure.");
            }

            if (rawArguments.Length == 0)
            {
                // Do nothing if there is no argument
                return;
            }

            if (rawArguments[0] == "-l")
            {
                // List all devices
                List<MinerDevice> deviceList = minerManager.GetAllMinerDevices();
                List<MinerDeviceOutput> deviceOutputList = new List<MinerDeviceOutput>();
                foreach(MinerDevice device in deviceList)
                {
                    MinerDeviceOutput outp = new MinerDeviceOutput(device);
                    deviceOutputList.Add(outp);
                }

                string output = JsonConvert.SerializeObject(deviceOutputList);
                Console.WriteLine(output);
            }
            else if (rawArguments[0] == "-d")
            {
                if (rawArguments.Length < 2)
                {
                    Console.WriteLine("{ 'result':'-1', 'error':'Argument Error.'}");
                    return;
                }

                long deviceId = 0;
                if (!Int64.TryParse(rawArguments[1], out deviceId))
                {
                    Console.WriteLine("{ 'result':'-1', 'error':'Cannot Parse DeviceId.'}");
                    return;
                }

                // Update the selected Device Id

                MinerConfig config = MinerConfig.ReadFromFile();
                config.SelectedDeviceId = deviceId;
                config.SaveToFile();

                Console.WriteLine("{ 'result':'0' }");
                return;

            }
        }

    }
}
