using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon
{
    public class MinerConfig
    {
        private static readonly string defaultConfigFileName = @"miner-config.json";
    

        public MinerConfig()
        {

        }

        public MinerConfig(MinerConfigFile configFile)
        {
            if (configFile == null)
            {
                return;
            }

            this.Version = configFile.version;
            this.IsFakeRun = configFile.is_fake_run;
            this.WalletAddress = configFile.wallet_address;
            this.PoolAddress = configFile.pool_address;

        }

        #region Properties for Config

        public string Version
        {
            get; private set;
        }

        public bool IsFakeRun
        {
            get; private set;
        }

        public string WalletAddress
        {
            get; private set;
        }

        public string PoolAddress
        {
            get; private set;
        }

        #endregion

        public static MinerConfig ReadFromFile()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            using (StreamReader sr = new StreamReader(directoryPath + "/" + defaultConfigFileName))
            {
                string jsonString = sr.ReadToEnd();
                MinerConfigFile configFile = Newtonsoft.Json.JsonConvert.DeserializeObject<MinerConfigFile>(jsonString);

                MinerConfig config = new MinerConfig(configFile);
                return config;
            }
        }

        public void SaveToFile()
        {
            MinerConfigFile configFile = new MinerConfigFile();
            configFile.version = this.Version;
            configFile.is_fake_run = this.IsFakeRun;
            configFile.pool_address = this.PoolAddress;
            configFile.wallet_address = this.WalletAddress;

            using (StreamWriter sw = new StreamWriter(defaultConfigFileName))
            {
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(configFile);
                sw.Write(content);
            }
        }


    };
    
    public class MinerConfigFile
    {
        public MinerConfigFile()
        {

        }

        public class MinerConfigDevice
        {
            public MinerConfigDevice()
            {

            }

            public string id;
            public string display_name;
            public string driver_version;
        };


        public string version;
        public string pool_address;
        public string wallet_address;
        public string machine_name;
        public string ip_address;
        public bool is_fake_run;

        public List<MinerConfigDevice> device_list;
    };
}
