using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemonTest.ComponentTests
{
    [TestClass]
    public class ListCommandTest : TestBase
    {
        [TestMethod]
        public void TestListSingle()
        {
            string listOutput = ExecuteDaemon("-l");
            Assert.IsTrue(listOutput.StartsWith("0|"), "List command output should start with 0|");

            listOutput = ExecuteDaemon("--List");
            Assert.IsTrue(listOutput.StartsWith("0|"), "List command output should start with 0|");
        }
    }
}
