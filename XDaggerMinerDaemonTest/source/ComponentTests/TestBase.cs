using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDaggerMiner.Common;
using XDaggerMinerDaemonTest.Utils;

namespace XDaggerMinerDaemonTest.ComponentTests
{
    public class TestBase
    {
        public static readonly string XDaggerServiceName = "XDaggerMinerWin";
        public static readonly string XDaggerEthServiceName = "XDaggerEthMinerWin";
        

        public static string ExecuteDaemon(string parameters)
        {
            string daemonFileFullPath = Path.Combine(Environment.CurrentDirectory, @"..\\XDaggerMinerWin\\XDaggerMinerDaemon.exe");

            Process daemonProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            startInfo.FileName = daemonFileFullPath;
            startInfo.Arguments = parameters;

            daemonProcess.StartInfo = startInfo;
            try
            {
                daemonProcess.Start();

                return daemonProcess.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ParseResultCode(string resultString)
        {
            if (!resultString.Contains("||"))
            {
                return -1;
            }

            string codeString = resultString.Split(new string[] { "||" }, StringSplitOptions.None)[0];
            int code = -1;
            Int32.TryParse(codeString, out code);

            return code;
        }

        public T ParseResultData<T>(string resultString)
        {
            if (!resultString.Contains("||"))
            {
                return default(T);
            }

            string dataString = resultString.Split(new string[] { "||" }, StringSplitOptions.None)[1];

            return JsonConvert.DeserializeObject<T>(dataString);
        }

        public static MinerConfig ReadConfigFile()
        {
            return MinerConfig.ReadFromFile(@"..\XDaggerMinerWin\miner-config.json");
        }

        public bool IsServiceExist(string serviceName, int instanceId)
        {
            string serviceFullName = (instanceId == 0) ? serviceName : string.Format("{0}_{1}", serviceName, instanceId);
            ServiceController[] services = ServiceController.GetServices("localhost");
            ServiceController serviceController = services.FirstOrDefault(s => s.ServiceName == serviceFullName);

            return (serviceController != null);
        }

        public static void UninstallAllServices()
        {
            string[] serviceNames = new string[] { "XDaggerMinerService", "XDaggerEthMinerService" };
            
            foreach (string serviceName in serviceNames)
            {
                for (int i = 0; i < 20; i++)
                {
                    try
                    {
                        string serviceFullName = (i == 0) ? serviceName : string.Format("{0}_{1}", serviceName, i);
                        TryUninstallService(serviceFullName, i);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        protected static void TryUninstallService(string serviceName, int instanceId, bool shouldThrow = false)
        {
            ServiceController[] services = ServiceController.GetServices("localhost");
            ServiceController serviceController = services.FirstOrDefault(s => s.ServiceName == serviceName);

            if (serviceController == null)
            {
                return;
            }

            string serviceBinaryFullPath = serviceController.ImagePath;

            if (string.IsNullOrEmpty(serviceBinaryFullPath))
            {
                return;
            }

            string folderFullPath = serviceBinaryFullPath.Substring(0, serviceBinaryFullPath.LastIndexOf('\\'));

            try
            {
                // Uninstall the service
                if (instanceId > 0)
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", "/instance=" + instanceId, serviceBinaryFullPath });
                }
                else
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", serviceBinaryFullPath });
                }
            }
            catch(Exception ex)
            {
                if (shouldThrow)
                {
                    throw;
                }
            }
        }
    }
}
