using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;
using XDaggerMiner.Common.Contracts.CommandOutputs;

namespace XDaggerMinerDaemonTest.ComponentTests
{
    [TestClass]
    public class ConfigureCommandTest : TestBase
    {
        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            UninstallAllServices();
        }

        [TestMethod]
        public void TestConfigureXDagger()
        {
            string result = ExecuteDaemon("-c \"{ 'XDaggerWallet':'1Nwa0TCr5umw5ZLAvKmHCl+SJDP21dyL', 'XDaggerPoolAddress':'xey.ti:13654', 'DeviceId':'p_0_d_0','AutoDecideInstanceId':'true' }\"");
            Assert.AreEqual(0, ParseResultCode(result), "Configure command should success.");
            
            ConfigureOutput output = ParseResultData<ConfigureOutput>(result);
            Assert.IsNotNull(output?.InstanceId, "InstanceId should not be null.");
        }

        [TestMethod]
        public void TestConfigureEth()
        {
            string result = ExecuteDaemon("-c \"{ 'EthPoolAddress':'stratum1tcp://0x73037FE73337D16D799c64632C1c79C19C8A85E6eth.f2pool.com:8008', 'DeviceId':'p_0_d_0', 'AutoDecideInstanceId':'true' }\"");
            Assert.AreEqual(0, ParseResultCode(result), "Configure command should success.");

            ConfigureOutput output = ParseResultData<ConfigureOutput>(result);
            Assert.AreEqual(0, output.InstanceId, "InstanceId should be 0.");
        }

        [TestMethod]
        [Ignore]
        public void TestConfigureAnotherInstanceId()
        {
            string result = ExecuteDaemon("-c \"{ 'XDaggerWallet':'1Nwa0TCr5umw5ZLAvKmHCl+SJDP21dyL', 'XDaggerPoolAddress':'xey.ti:13654', 'DeviceId':'p_0_d_0','AutoDecideInstanceId':'true' }\"");
            Assert.AreEqual(0, ParseResultCode(result), "Configure command should success.");

            try
            {
                result = ExecuteDaemon("-s install");
                Assert.AreEqual(0, ParseResultCode(result), "Service should be installed.");

                Assert.IsTrue(IsServiceExist(XDaggerServiceName, 0), "Found service in Windows Services.");

                result = ExecuteDaemon("-c \"{ 'AutoDecideInstanceId':'true' }\"");
                ConfigureOutput output = ParseResultData<ConfigureOutput>(result);

                Assert.AreEqual(output.InstanceId, 1, "InstanceId should be 1.");

                MinerConfig config = ReadConfigFile();
                Assert.IsTrue(config.InstanceType == MinerConfig.InstanceTypes.XDagger, "InstanceType is XDagger.");
                Assert.AreEqual(config.InstanceId, 1, "InstanceId in config should be 1.");
            }
            finally
            {
                TryUninstallService("XDaggerMinerWin", 0);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestConfigureUpdateInstanceType()
        {
            string result = ExecuteDaemon("-c \"{ 'XDaggerWallet':'1Nwa0TCr5umw5ZLAvKmHCl+SJDP21dyL', 'XDaggerPoolAddress':'xey.ti:13654', 'DeviceId':'p_0_d_0','AutoDecideInstanceId':'true' }\"");
            Assert.AreEqual(ParseResultCode(result), 0, "Configure command should success.");

            try
            {
                result = ExecuteDaemon("-s install");
                Assert.AreEqual(ParseResultCode(result), 0, "Service should be installed.");

                Assert.IsTrue(IsServiceExist(XDaggerServiceName, 0), "Found service in Windows Services.");

                result = ExecuteDaemon("-c \"{ 'EthPoolAddress':'stratum1tcp://0x73037FE73337D16D799c64632C1c79C19C8A85E6eth.f2pool.com:8008', 'AutoDecideInstanceId':'true' }\"");

                Assert.IsTrue(IsServiceExist(XDaggerServiceName, 0), "Service is not changed after configuration command.");


                ConfigureOutput output = ParseResultData<ConfigureOutput>(result);

                Assert.AreEqual(output.InstanceId, 0, "InstanceId should be 0.");

                MinerConfig config = ReadConfigFile();
                Assert.IsTrue(config.InstanceType == MinerConfig.InstanceTypes.XDagger, "InstanceType is XDagger.");
                Assert.AreEqual(config.InstanceId, 0, "InstanceId in config should be 0.");
                Assert.IsTrue(config.UpdatedInstanceType == MinerConfig.InstanceTypes.Eth, "UpdatedInstanceType is XDagger.");
                Assert.AreEqual(config.UpdatedInstanceId, 0, "UpdatedInstanceId in config should be 0.");

                result = ExecuteDaemon("-s start");
                Assert.IsFalse(IsServiceExist(XDaggerServiceName, 0), "XDagger service should be uninstalled.");
                Assert.IsTrue(IsServiceExist(XDaggerEthServiceName, 0), "XDagger Eth service should be installed.");

                config = ReadConfigFile();
                Assert.IsTrue(config.InstanceType == MinerConfig.InstanceTypes.Eth, "InstanceType is XDagger.");
                Assert.AreEqual(config.InstanceId, 0, "InstanceId in config should be 0.");
                Assert.IsTrue(config.UpdatedInstanceType == null, "UpdatedInstanceType is null.");
                Assert.AreEqual(config.UpdatedInstanceId, null, "UpdatedInstanceId in config should be null.");
            }
            finally
            {
                TryUninstallService("XDaggerMinerWin", 0);
            }
        }
    }
}
