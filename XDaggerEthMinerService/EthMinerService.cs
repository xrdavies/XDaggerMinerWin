using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XDaggerMinerDaemon;

namespace XDaggerEthMinerService
{
    public partial class EthMinerService : ServiceBase
    {
        MinerConfig minerConfig = null;

        Process ehtMinerProcess = null;

        System.Diagnostics.EventLog implementEventLog = null;

        public EthMinerService()
        {
            InitializeComponent();
            minerConfig = MinerConfig.ReadFromFile();

            
        }

        protected override void OnStart(string[] args)
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            string ethExecutionFileFullPath = Path.Combine(directoryPath, "External/ethminer.exe");

            // log4net.GlobalContext.Properties["LogFilePath"] = directoryPath; //log file path
            // log4net.Config.XmlConfigurator.Configure();

            ehtMinerProcess = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            startInfo.FileName = ethExecutionFileFullPath;
            startInfo.Arguments = "-G stratum1+tcp://73037FE73337D16D799c64632C1c79C19C8A85E6@eth-ar.dwarfpool.com:8008/7Fb21ac4Cd75d9De3E1c5D11D87bB904c01880fc/charlie_5899@hotmail.com";
            ehtMinerProcess.StartInfo = startInfo;

            try
            {
                ehtMinerProcess.Start();
                //// ehtMinerProcess.ErrorDataReceived += Process_ErrorDataReceived;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            string sourceName = "XDaggerEthMiner";
            string logName = "XDaggerMinerLog";

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
            
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerWork);
            //// timer.Start();
        }

        private void OnTimerWork(object sender, ElapsedEventArgs e)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("Test");

            log.Debug("OnTimerWork");

            this.implementEventLog.WriteEntry("OnTimerWork");

            StreamReader reader = ehtMinerProcess.StandardError;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                log.Debug(line);
                this.implementEventLog.WriteEntry(line);

                Thread.Sleep(30);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.implementEventLog.WriteEntry(e.Data);

        }

        protected override void OnStop()
        {
            if (ehtMinerProcess != null && !ehtMinerProcess.HasExited)
            {
                try
                {
                    ehtMinerProcess.Kill();
                }
                catch(Exception)
                {
                    // Swallow the exceptions
                }
            }
        }
    }
}
