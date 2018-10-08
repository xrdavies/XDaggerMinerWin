using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XDaggerMiner.Common;
using XDaggerMiner.Common.Contracts;
using XDaggerMiner.Common.Utils;
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
        }

        public CommandResult Execute()
        {
            try
            {
                minerConfig = MinerConfig.ReadFromFile();
            }
            catch (TargetExecutionException ex)
            {
                logger.WriteLog(3, 0, "Read config file failed: " + ex.Message);
                return CommandResult.ErrorResult(ex);
            }

            if (minerConfig == null || minerManager == null)
            {
                logger.WriteLog(3, 0, "Cannot Execute due to previous initialization failure.");
                return CommandResult.ErrorResult(DaemonErrorCode.INITIALIZE_FAILED, "Cannot Execute due to previous initialization failure.");
            }

            List<CommandInstance> commands = ConsoleCommand.ParseCommands(rawArguments);
            if (commands.Count == 0)
            {
                // Do nothing is there is no command
                return CommandResult.ErrorResult(DaemonErrorCode.COMMAND_MISSING, "Missing command here.");
            }

            if (commands.Count > 1)
            {
                // Not Supported
                return CommandResult.ErrorResult(DaemonErrorCode.COMMAND_TOO_MANY, "Curent not supporting multiple commands.");
            }

            
            // Disable the Console output during the command call
            TextWriter defaultOutput = Console.Out;

            try
            {
                Console.SetOut(TextWriter.Null);
                return commands[0].Execute();
            }
            catch(TargetExecutionException ex)
            {
                return CommandResult.ErrorResult(ex);
            }
            catch(Exception ex)
            {
                return CommandResult.ErrorResult(DaemonErrorCode.UNKNOWN_ERROR, ex.Message);
            }
            finally
            {
                Console.SetOut(defaultOutput);
            }
        }
    }
}
