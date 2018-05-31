using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XDaggerMinerRuntimeCLI;


namespace XDaggerMinerDaemon
{
    public class ConsoleLogger : LoggerBase
    {
        public override void WriteLog(int level, int eventId, string message)
        {
            string levelString = string.Empty;
            switch (level)
            {
                case 0:
                    levelString = "Information";
                    break;
                case 1:
                    levelString = "Warning";
                    break;
                case 2:
                    levelString = "Error";
                    break;
                case 3:
                    levelString = "Fatal";
                    break;
                default:
                    levelString = "Unknown";
                    break;
            }

            Console.WriteLine(string.Format("[{0}][{1}] {2}", levelString, eventId, message));
        }
    }
}
