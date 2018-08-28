using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using XDaggerMinerRuntimeCLI;


namespace XDaggerMiner.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class PrimaryLogger : LoggerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private static PrimaryLogger instance = null;

        public static PrimaryLogger GetInstance()
        {
            if (instance == null)
            {
                instance = new PrimaryLogger();
            }

            return instance;
        }

        public override void WriteLog(int level, int eventId, string message)
        {
            string formattedMessage = string.Format("[{0}]{1}", eventId, message);

            log4net.ILog log = log4net.LogManager.GetLogger(PrimaryLogger.RetrieveCallerMethod());
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
        }

        public override void WriteTrace(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(PrimaryLogger.RetrieveCallerMethod());
            log.Debug(message);
        }

        private static string RetrieveCallerMethod()
        {
            int stackTraceIndex = 1;
            string fullMethodName = string.Empty;

            while (true)
            {
                try
                {
                    StackFrame frm = new StackFrame(stackTraceIndex);
                    MethodInfo mi = (MethodInfo)frm.GetMethod();
                    if (mi.DeclaringType != System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
                         && mi.Name != "WriteLog")
                    {
                        string methodSignature = mi.ToString();
                        methodSignature = methodSignature.Substring(methodSignature.IndexOf(' ') + 1);
                        fullMethodName = string.Format("{0}.{1}", mi.DeclaringType, methodSignature);
                        break;
                    }

                    stackTraceIndex++;
                }
                catch (Exception)
                {
                    // Just break
                    break;
                }
            }

            return fullMethodName;
        }
    }
}
