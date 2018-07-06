using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerDaemon;
using XDaggerMinerDaemon.Commands;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon
{
    public class Program
    {

        static void Main(string[] args)
        {
            MainConsole console = new MainConsole(args);
            CommandResult result = console.Execute();

            Console.WriteLine(result?.ToString());
        }
    };

}
