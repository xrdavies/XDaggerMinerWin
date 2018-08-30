using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerService
{
    public class MinerEventLog : LoggerBase
    {
        private static readonly string SourceName = "XDaggerMiner";
        private static readonly string LogName = "XDaggerMinerLog";

        private System.Diagnostics.EventLog implementEventLog;

        private string instanceId = string.Empty;

        public MinerEventLog(string instanceId = "")
        {
            if (!string.IsNullOrWhiteSpace(instanceId))
            {
                this.instanceId = instanceId;
            }

            string sourceName = string.IsNullOrEmpty(this.instanceId) ? SourceName : string.Format("{0}_{1}", SourceName, this.instanceId);
            string logName = string.IsNullOrEmpty(this.instanceId) ? LogName : string.Format("{0}_{1}", LogName, this.instanceId);

            this.implementEventLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.implementEventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.implementEventLog)).EndInit();

            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    sourceName, logName);
            }

            this.implementEventLog.Source = sourceName;
            this.implementEventLog.Log = logName;
        }
        
        public override void WriteLog(int level, int eventId, string message)
        {
            string formattedMessage = string.Format("[{0}]{1}", eventId, message);

            log4net.ILog log = log4net.LogManager.GetLogger("Service");
            switch (level)
            {
                case 0:
                    // Trace
                    log.Debug(formattedMessage);
                    break;
                case 1:
                    // Error
                    log.Error(formattedMessage);
                    break;
                case 2:
                    // Warning
                    log.Warn(formattedMessage);
                    break;
                case 3:
                    // Information
                    log.Info(formattedMessage);
                    break;
                default:
                    log.Fatal(formattedMessage);
                    break;
            }

            if (level == 0)
            {
                // If this is trace, just return
                return;
            }

            EventLogEntryType entryType;
            switch (level)
            {
                case 1:
                    entryType = EventLogEntryType.Error;
                    break;
                case 2:
                    entryType = EventLogEntryType.Warning;
                    break;
                case 3:
                    entryType = EventLogEntryType.Information;
                    break;
                default:
                    entryType = EventLogEntryType.Information;
                    break;
            };

            this.implementEventLog.WriteEntry(message, entryType, eventId);
        }
    }
}
