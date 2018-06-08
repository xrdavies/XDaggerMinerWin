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

        public void Execute()
        {
            if (this.command != null)
            {
                this.command.Execute(this.Parameter);
            }
        }

    }
}
