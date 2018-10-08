using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMiner.Common.Contracts.CommandOutputs
{
    public class MinerDeviceOutput
    {
        public MinerDeviceOutput()
        {

        }

        public MinerDeviceOutput(MinerDevice device)
        {
            this.DeviceId = device.GetDeviceId();
            this.DisplayName = device.GetDisplayName();
            this.DeviceVersion = device.GetDeviceVersion();
            this.DriverVersion = device.GetDriverVersion();
        }

        public string DeviceId
        {
            get; private set;
        }


        public string DisplayName
        {
            get; private set;
        }

        public string DeviceVersion
        {
            get; private set;
        }

        public string DriverVersion
        {
            get; private set;
        }
    }
}
