using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMiner.Common.Contracts
{
    public class MinerServiceState
    {
        public const string Unknown = "unknown";

        public const string Idle = "idle";

        public const string Initialzing = "initializing";

        public const string Connected = "connected";

        public const string Disconnected = "disconnected";

        public const string Mining = "mining";

        public const string Error = "error";
    }
}
