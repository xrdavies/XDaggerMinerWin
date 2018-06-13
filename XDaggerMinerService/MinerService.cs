using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using XDaggerMinerDaemon;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerService
{
    public partial class MinerService : ServiceBase
    {
        private MinerEventLog minerEventLog;

        private MinerManager minerManager;

        private MinerConfig config;


        public MinerService()
        {
            InitializeComponent();

            config = MinerConfig.ReadFromFile();

            minerEventLog = new MinerEventLog();
            minerManager = new MinerManager(config.IsFakeRun);
            minerManager.SetLogger(minerEventLog);

        }

        protected override void OnStart(string[] args)
        {
            
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerWork);
            timer.Start();


            minerEventLog.WriteInformation(0, "XDaggerMiner Service Started.");

        }

        protected override void OnStop()
        {
            minerEventLog.WriteInformation(0, "XDaggerMiner Service Stopped.");

        }



        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            string defaultConfigFileName = directoryPath + "/miner-config.json";
            using (StreamReader sr = new StreamReader(defaultConfigFileName))
            {
                string jsonString = sr.ReadToEnd();
                minerEventLog.WriteInformation(0, "Triggered by Timer." + jsonString);
            }
            
        }

        private void OnTimerWork(object sender, ElapsedEventArgs e)
        {
            minerEventLog.WriteInformation(0, "Triggered by Timer.");

            minerManager.DoMining(config.PoolAddress, config.WalletAddress);

        }
    }
}
