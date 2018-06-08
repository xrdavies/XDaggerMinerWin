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
            this.Device = MinerConfigDevice.CreateFromFile(configFile.device);
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
            get; set;
        }

        public string PoolAddress
        {
            get; private set;
        }

        public long SelectedDeviceId
        {
            get; set;
        }

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
                MinerConfigFile configFile = JsonConvert.DeserializeObject<MinerConfigFile>(jsonString);

                MinerConfig config = new MinerConfig(configFile);
                return config;
            }
        }

        public void SaveToFile()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            MinerConfigFile configFile = new MinerConfigFile();
            configFile.version = this.Version;
            configFile.is_fake_run = this.IsFakeRun;
            configFile.pool_address = this.PoolAddress;
            configFile.wallet_address = this.WalletAddress;
            configFile.device = this.Device?.ToConfigFile();

            using (StreamWriter sw = new StreamWriter(Path.Combine(directoryPath, defaultConfigFileName)))
            {
                string content = JsonConvert.SerializeObject(configFile, Formatting.Indented);
                sw.Write(content);
            }
        }
        
    };

    public class MinerConfigDevice
    {
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

        public MinerConfigFile.MinerConfigFileDevice ToConfigFile()
        {
            MinerConfigFile.MinerConfigFileDevice deviceFile = new MinerConfigFile.MinerConfigFileDevice();
            deviceFile.id = this.DeviceId;
            deviceFile.display_name = this.DisplayName;
            deviceFile.device_version = this.DeviceVersion;
            deviceFile.driver_version = this.DriverVersion;

            return deviceFile;
        }

        public string DeviceId
        {
            get; private set;
        }

        public string DisplayName
        {
            get; private set;
        }

        public string DeviceVersion
        {
            get; private set;
        }

        public string DriverVersion
        {
            get; private set;
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
