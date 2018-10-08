using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMiner.Common.Contracts.CommandOutputs
{
    public class ReportOutput
    {
        /// <summary>
        /// Status: Unknown, NotInstalled, Stopped, Disconnected, Connected, Mining
        /// </summary>
        public enum StatusEnum
        {
            Unknown = 0,
            NotInstalled = 10,
            Stopped = 20,
            Initializing = 30,
            Disconnected = 40,
            Connected = 50,
            Mining = 60,
            Error = 99,
        }

        public StatusEnum Status
        {
            get; set;
        }

        public string Details
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
