using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon
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
        }

        public long DeviceId
        {
            get; private set;
        }


        public string DisplayName
        {
            get; private set;
        }

    }
}
