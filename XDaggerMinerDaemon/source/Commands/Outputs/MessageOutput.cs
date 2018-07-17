using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Commands.Outputs
{
    public class MessageOutput
    {
        public static MessageOutput Create(string msg)
        {
            MessageOutput output = new MessageOutput();
            output.Message = msg;

            return output;
        }
        public string Message
        {
            get; set;
        }
    }
}
