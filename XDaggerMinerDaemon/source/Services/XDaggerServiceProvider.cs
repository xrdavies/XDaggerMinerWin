using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Services
{
    public class XDaggerServiceProvider : ServiceProvider
    {
        protected override string GetServiceNameBase()
        {
            return "XDaggerMinerService";
        }

        protected override string GetServiceBinaryName()
        {
            return "XDaggerMinerService.exe";
        }

        protected override string GetNamedPipeNameTemplate()
        {
            return "XDaggerMinerPipe_{0}";
        }
    }
}
