using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMiner.Common.Contracts.CommandOutputs
{
    public class ConfigureOutput
    {
        public static ConfigureOutput Create(int? instanceId)
        {
            ConfigureOutput output = new ConfigureOutput();
            output.InstanceId = instanceId;

            return output;
        }
        public int? InstanceId
        {
            get; set;
        }

    }
}
