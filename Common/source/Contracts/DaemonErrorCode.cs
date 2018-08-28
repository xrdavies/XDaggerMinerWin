using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMiner.Common.Contracts
{
    public enum DaemonErrorCode
    {
        // Common Error
        UNKNOWN_ERROR = 30000,
        INITIALIZE_FAILED = 30005,

        COMMAND_NOT_FOUND = 30010,
        COMMAND_MISSING = 30015,
        COMMAND_TOO_MANY = 30016,

        COMMAND_FORMAT_ERROR = 30020,
        COMMAND_PARAM_ERROR = 30030,
        COMMAND_TIME_OUT = 30035,

        // Common CL Error
        CL_DEVICE_ERROR = 30040,
        CL_PERMISSION_ERROR = 30050,

        CONFIG_FILE_MISSING = 30060,
        CONFIG_FILE_ERROR = 30065,



        // Configure Command
        CONFIG_DEVICE_NOT_FOUND = 50060,
        CONFIG_WALLET_FORMET_ERROR = 50070,
        CONFIG_WALLET_NOT_FOUND = 50080,
        CONFIG_ONLY_ONE_INSTANCE_TYPE_ALLOWED = 50090,

        // Service Command
        SERVICE_NOT_INSTALLED = 51080,


        // Report Command
        REPORT_NAMEDPIPE_ERROR = 52090,

    }
}
