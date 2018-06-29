using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Commands.Outputs
{
    public class ReportOutput
    {
        /// <summary>
        /// Status: Unknown, NotInstalled, Stopped, Disconnected, Connected, Mining
        /// </summary>
        public enum StatusEnum
        {
            Unknown = 0,
            NotInstalled = 1,
            Stopped = 2,
            Disconnected = 3,
            Connected = 4,
            Mining = 5
        }

        public StatusEnum Status
        {
            get; set;
        }

        public string StatusString
        {
            get
            {
                return this.Status.ToString();
            }
        }

        /// <summary>
        /// Unit: Mhash per second
        /// </summary>
        public double HashRate
        {
            get; set;
        }


    }
}
