using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XDaggerMiner.Common
{
    public class MinerConfig
    {
        public enum InstanceTypes
        {
            Unset = 0,
            XDagger = 1,
            Eth = 2,
        }

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

        #region Properties for Config

        [JsonProperty(PropertyName = "version")]
        public string Version
        {
            get; set;
        }

        [JsonProperty(PropertyName = "instance_id")]
        public int InstanceId
        {
            get; set;
        }

        /// <summary>
        /// Instance Type
        /// </summary>
        [JsonProperty(PropertyName = "instance_type")]
        public InstanceTypes InstanceType
        {
            get; set;
        }

        [JsonProperty(PropertyName = "updated_instance_id")]
        public int? UpdatedInstanceId
        {
            get; set;
        }

        /// <summary>
        /// Instance Type
        /// </summary>
        [JsonProperty(PropertyName = "updated_instance_type")]
        public InstanceTypes? UpdatedInstanceType
        {
            get; set;
        }

        [JsonProperty(PropertyName = "is_fake_run")]
        public bool IsFakeRun
        {
            get; set;
        }

        [JsonProperty(PropertyName = "xdagger")]
        public XDaggerMinerConfig XDaggerMiner
        {
            get; set;
        }

        [JsonProperty(PropertyName = "eth")]
        public EthMinerConfig EthMiner
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
        public MinerConfigDevice()
        {

        }

        public MinerConfigDevice(string deviceId, string displayName, string deviceVersion, string driverVersion)
        {
            this.Id = deviceId;
            this.DisplayName = displayName;
            this.DeviceVersion = deviceVersion;
            this.DriverVersion = driverVersion;
        }

        private string id = string.Empty;
        private int platformId = 0;
        private int deviceId = 0;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;

                string deviceIdFormat = @"^p_(?<pId>[0-9]+)_d_(?<dId>[0-9]+)$";
                MatchCollection matches = Regex.Matches(id, deviceIdFormat);
                if (matches.Count > 0)
                {
                    Int32.TryParse(matches[0].Groups["pId"].Value, out platformId);
                    Int32.TryParse(matches[0].Groups["dId"].Value, out deviceId);
                }
            }
        }

        [JsonProperty(PropertyName = "platform_id")]
        public int PlatformId
        {
            get
            {
                return platformId;
            }
        }

        [JsonProperty(PropertyName = "device_id")]
        public int DeviceId
        {
            get
            {
                return deviceId;
            }
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

    public class XDaggerMinerConfig
    {
        [JsonProperty(PropertyName = "wallet_address")]
        public string WalletAddress
        {
            get; set;
        }

        [JsonProperty(PropertyName = "pool_address")]
        public string PoolAddress
        {
            get; set;
        }
    }

    public class EthMinerConfig
    {
        [JsonProperty(PropertyName = "pool_address")]
        public string PoolAddress
        {
            get; set;
        }

        [JsonProperty(PropertyName = "gpu_type")]
        public string GPUType
        {
            get; set;
        }
    }
}
