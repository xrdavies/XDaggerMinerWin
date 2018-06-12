using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Commands
{
    /// <summary>
    /// This is an instance of Command that created from Console command line.
    /// </summary>
    public class CommandInstance
    {
        private ConsoleCommand command;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public CommandInstance(ConsoleCommand command)
        {
            this.command = command;
        }

        public string Parameter
        {
            get; set;
        }

        public CommandResult Execute()
        {
            if (this.command != null)
            {
                return this.command.Execute(this.Parameter);
            }

            return CommandResult.ErrorResult(101, "Cannot Trigger Command.");
        }
    }
}
