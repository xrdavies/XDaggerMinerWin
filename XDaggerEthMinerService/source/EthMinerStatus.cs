using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XDaggerMiner.Common.Contracts;

namespace XDaggerEthMinerService
{
    public class EthMinerStatus
    {
        public static readonly TimeSpan StatusExpiration = TimeSpan.FromSeconds(30);

        public DateTime LastUpdatedTime
        {
            get; private set;
        }

        public EthMinerStatus()
        {
            this.LastUpdatedTime = DateTime.UtcNow;
        }

        public bool HasExpired()
        {
            return LastUpdatedTime.Add(StatusExpiration) < DateTime.UtcNow;
        }

        private string minerStatus = MinerServiceState.Unknown;
        public string MinerStatus
        {
            get
            {
                if (minerStatus != MinerServiceState.Error && HasExpired())
                {
                    return MinerServiceState.Unknown;
                }

                return minerStatus;
            }
            set
            {
                this.LastUpdatedTime = DateTime.UtcNow;
                this.minerStatus = value;
            }
        }

        private int hashRate = 0;
        public int HashRate
        {
            get
            {
                if (HasExpired())
                {
                    return 0;
                }

                return hashRate;
            }
            set
            {
                this.LastUpdatedTime = DateTime.UtcNow;
                this.hashRate = value;
            }
        }

        private string details = string.Empty;
        public string Details
        {
            get
            {
                if (HasExpired())
                {
                    return string.Empty;
                }

                return details;
            }
            set
            {
                this.LastUpdatedTime = DateTime.UtcNow;
                this.details = value;
            }
        }

        public void HandleOutputMessage(string messageLine)
        {
            if (MinerStatus == MinerServiceState.Error)
            {
                // Ignore other output when there is error
                return;
            }

            if (messageLine.Contains("Unknown URI scheme unspecified"))
            {
                MinerStatus = MinerServiceState.Idle;
                HashRate = 0;
                return;
            }

            if (messageLine.Contains("Found suitable OpenCL device"))
            {
                MinerStatus = MinerServiceState.Initialzing;
                HashRate = 0;
                return;
            }

            if (messageLine.Contains("Connected to "))
            {
                MinerStatus = MinerServiceState.Connected;
                HashRate = 0;
                return;
            }

            if (messageLine.Contains("CL_INVALID_BUFFER_SIZE"))
            {
                MinerStatus = MinerServiceState.Error;
                Details = "CL_INVALID_BUFFER_SIZE";
                HashRate = 0;
                return;
            }

            string speedPattern = @"Speed   (?<hashrate>[0-9\.]+) Mh/s    gpu\/(?<gpu>[0-9]+)  (?<gpurate>[0-9\.]+)";
            MatchCollection matches = Regex.Matches(messageLine, speedPattern);
            if (matches.Count > 0)
            {
                MinerStatus = MinerServiceState.Mining;

                Match match = matches[0];

                double hashRateMhs = 0;
                if (Double.TryParse(match.Groups["hashrate"].Value, out hashRateMhs))
                {
                    HashRate = (int)(hashRateMhs * 1000000);
                }

                int gpuCount = 0;
                Int32.TryParse(match.Groups["gpu"].Value, out gpuCount);

                double gpuRate = 0;
                Double.TryParse(match.Groups["gpurate"].Value, out gpuRate);
                return;
            }
        }
    }
}
