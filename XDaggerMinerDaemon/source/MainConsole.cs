using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XDaggerMinerDaemon.Commands;
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

            List<CommandInstance> commands = ConsoleCommand.ParseCommands(rawArguments);
            if (commands.Count == 0)
            {
                // Do nothing is there is no command
                return;
            }

            if (commands.Count > 1)
            {
                // Not Supported
                Console.WriteLine("{ 'result':'-1', 'error':'Curent not supporting multiple commands.'}");
                return;
            }

            commands[0].Execute();
            
            if (rawArguments[0] == "-l")
            {
                
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
