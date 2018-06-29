using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Commands
{
    public abstract class ConsoleCommand
    {
        public abstract string GetShortName();

        public abstract string GetLongName();

        public abstract CommandInstance GenerateInstance(string[] arguments, ref int nextIndex);

        public abstract CommandResult Execute(string parameter);

        public virtual void Validate()
        {

        }

        public bool IsMatchName(string commandName)
        {
            return commandName.Equals(GetShortName(), StringComparison.InvariantCultureIgnoreCase)
                || commandName.Equals(GetLongName(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static CommandInstance ReadCommand(string[] arguments, ref int nextIndex)
        {
            if (arguments.Length == 0 || arguments.Length <= nextIndex)
            {
                return null;
            }

            List<ConsoleCommand> availableCommandList = new List<ConsoleCommand>();
            availableCommandList.Add(new ListDevicesCommand());
            availableCommandList.Add(new ConfigureCommand());
            availableCommandList.Add(new ServiceCommand());
            availableCommandList.Add(new ReportCommand());

            foreach (ConsoleCommand command in availableCommandList)
            {
                if (command.IsMatchName(arguments[nextIndex]))
                {
                    nextIndex++;
                    CommandInstance instance = command.GenerateInstance(arguments, ref nextIndex);
                    return instance;
                }
            }

            return null;

        }

        public static List<CommandInstance> ParseCommands(string[] arguments)
        {
            List<CommandInstance> resultInstanceList = new List<CommandInstance>();

            int index = 0;
            while (true)
            {
                CommandInstance instance = ReadCommand(arguments, ref index);
                if (instance == null)
                {
                    break;
                }

                resultInstanceList.Add(instance);
            }

            return resultInstanceList;
        }
    }
}
