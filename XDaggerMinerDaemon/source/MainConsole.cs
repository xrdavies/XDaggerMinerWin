using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon
{
    public class MainConsole
    {
        private MinerConfig minerConfig = null;

        private MinerManager minerManager = null;

        private LoggerBase logger = null;

        private string[] rawArguments;


        public MainConsole(string[] args)
        {
            rawArguments = args;

            minerManager = new MinerManager();
            logger = new ConsoleLogger();
            minerManager.SetLogger(logger);

            try
            {
                minerConfig = MinerConfig.ReadFromFile();

            }
            catch(Exception ex)
            {
                logger.WriteLog(3, 0, "Read config file failed: " + ex.ToString());
            }

            
        }

        public void Execute()
        {
            if (minerConfig == null || minerManager == null)
            {
                logger.WriteLog(3, 0, "Cannot Execute due to previous initialization failure.");
            }

            minerManager.DoMining(minerConfig.PoolAddress, minerConfig.WalletAddress);


        }

    }
}
