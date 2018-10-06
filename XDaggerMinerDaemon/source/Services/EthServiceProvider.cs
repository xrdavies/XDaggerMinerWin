using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Services
{
    public class EthServiceProvider : ServiceProvider
    {
        protected override string GetServiceNameBase()
        {
            return "XDaggerEthMinerService";
        }

        protected override string GetServiceBinaryName()
        {
            return "XDaggerEthMinerService.exe";
        }

        protected override string GetNamedPipeNameTemplate()
        {
            return "XDaggerEthMinerPipe_{0}";
        }
    }
}
