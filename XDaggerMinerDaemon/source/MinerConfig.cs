using Newtonsoft.Json;
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

        private static MinerConfig instance = null;

        public MinerConfig()
        {

        }

        public static MinerConfig GetInstance()
        {
            if (instance == null)
            {
                instance = ReadFromFile();
            }

            return instance;
        }

        /*
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
            this.Device = MinerConfigDevice.CreateFromFile(configFile.device);
        }
        */

        #region Properties for Config

        [JsonProperty(PropertyName = "version")]
        public string Version
        {
            get; set;
        }

        [JsonProperty(PropertyName = "instance")]
        public string InstanceId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "is_fake_run")]
        public bool IsFakeRun
        {
            get; set;
        }

        [JsonProperty(PropertyName = "wallet")]
        public string WalletAddress
        {
            get; set;
        }

        [JsonProperty(PropertyName = "pool_address")]
        public string PoolAddress
        {
            get; set;
        }

        [JsonProperty(PropertyName = "device")]
        public MinerConfigDevice Device
        {
            get; set;
        }

        #endregion

        public static MinerConfig ReadFromFile()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            using (StreamReader sr = new StreamReader(Path.Combine(directoryPath, defaultConfigFileName)))
            {
                string jsonString = sr.ReadToEnd();
                MinerConfig config = JsonConvert.DeserializeObject<MinerConfig>(jsonString);
                return config;
            }
        }

        public void SaveToFile()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);
            
            using (StreamWriter sw = new StreamWriter(Path.Combine(directoryPath, defaultConfigFileName)))
            {
                string content = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(content);
            }
        }
        
    };

    public class MinerConfigDevice
    {
        /*
        public static MinerConfigDevice CreateFromFile(MinerConfigFile.MinerConfigFileDevice deviceFile)
        {
            if (deviceFile == null)
            {
                return null;
            }

            MinerConfigDevice device = new MinerConfigDevice();
            device.DeviceId = deviceFile.id;
            device.DisplayName = deviceFile.display_name;
            device.DeviceVersion = deviceFile.device_version;
            device.DriverVersion = deviceFile.driver_version;

            return device;
        }
        */

        public MinerConfigDevice()
        {

        }

        public MinerConfigDevice(string deviceId, string displayName, string deviceVersion, string driverVersion)
        {
            this.DeviceId = deviceId;
            this.DisplayName = displayName;
            this.DeviceVersion = deviceVersion;
            this.DriverVersion = driverVersion;
        }

        /*
        public MinerConfigFile.MinerConfigFileDevice ToConfigFile()
        {
            MinerConfigFile.MinerConfigFileDevice deviceFile = new MinerConfigFile.MinerConfigFileDevice();
            deviceFile.id = this.DeviceId;
            deviceFile.display_name = this.DisplayName;
            deviceFile.device_version = this.DeviceVersion;
            deviceFile.driver_version = this.DriverVersion;

            return deviceFile;
        }
        */

        [JsonProperty(PropertyName = "id")]
        public string DeviceId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName
        {
            get; set;
        }

        [JsonProperty(PropertyName = "device_version")]
        public string DeviceVersion
        {
            get; set;
        }

        [JsonProperty(PropertyName = "driver_version")]
        public string DriverVersion
        {
            get; set;
        }
    }

    public class MinerConfigFile
    {
        public MinerConfigFile()
        {

        }

        public class MinerConfigFileDevice
        {
            public MinerConfigFileDevice()
            {

            }

            public string id = string.Empty;
            public string display_name = string.Empty;
            public string device_version = string.Empty;
            public string driver_version = string.Empty;
        };


        public string version = string.Empty;
        public string pool_address = string.Empty;
        public string wallet_address = string.Empty;
        public string machine_name = string.Empty;
        public string ip_address = string.Empty;
        public bool is_fake_run = true;
        public MinerConfigFileDevice device = null;
    };
}
