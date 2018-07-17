using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XDaggerMinerDaemon.Commands.Outputs;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerDaemon.Commands
{
    public class TestingCommands : ConsoleCommand
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(string value);

        [DllImport("XDaggerMinerRuntime.dll")]
        public static extern void DoWork([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        private MinerManager minerManager = null;

        private ConsoleLogger minerLog = null;

        public override CommandResult Execute(string parameter)
        {
            minerLog = new ConsoleLogger();
            minerManager = new MinerManager(false);
            minerManager.SetLogger(minerLog);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerWork);
            timer.Start();

            Thread.Sleep(3000);

            /*
            Task.Factory.StartNew(() => PingMiner());

            Thread.Sleep(3000);

            Task.Factory.StartNew(() => PingMiner());

            Thread.Sleep(3000);

            Task.Factory.StartNew(() => PingMiner());
            */

            MessageOutput message = MessageOutput.Create("Finished");
            return CommandResult.CreateResult(message);
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            CommandInstance instance = new CommandInstance(this);

            return instance;
        }

        public override string GetLongName()
        {
            return "--Test";
        }

        public override string GetShortName()
        {
            return "-t";
        }

        public void PingMiner()
        {
            minerManager.DoMining("pool.xdag.us:13654", "gKNRtSL1pUaTpzMuPMznKw49ILtP6qX3");
        }

        private void OnTimerWork(object sender, ElapsedEventArgs e)
        {
            //// minerManager.DoMining("pool.xdag.us:13654", "gKNRtSL1pUaTpzMuPMznKw49ILtP6qX3");
            ProgressCallback callback = ((value) => { minerLog.WriteLog(0, 0, value); });
            DoWork(callback);
        }
    }
}
