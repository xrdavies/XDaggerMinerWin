using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XDaggerMiner.Common;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerService
{
    
    public partial class MinerService : ServiceBase
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(string value);

        [DllImport("XDaggerMinerRuntime.dll")]
        public static extern void DoWork([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);
        

        private static readonly string NamedPipeServerNameTemplate = "XDaggerMinerPipe_{0}";

        private MinerEventLog minerEventLog;

        // private ConsoleLogger consoleLog;
        
        private MinerManager minerManager;

        private MinerConfig config;
        
        private Task namedPipeServerTask = null;

        public MinerService()
        {
            InitializeComponent();
            
            config = MinerConfig.ReadFromFile();
            minerManager = new MinerManager(config.IsFakeRun);

            minerEventLog = new MinerEventLog();
            minerManager.SetLogger(minerEventLog);

            minerEventLog.WriteLog(0, 0, "MinerService Started.");

            namedPipeServerTask = new Task(() =>
            {
                NamedPipeServerMain();
            });
        }

        protected override void OnStart(string[] args)
        {
            // First, configure the Miner
            ConfigureMiner();

            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerWork);
            timer.Start();

            if (namedPipeServerTask.Status != TaskStatus.Running)
            {
                namedPipeServerTask.Start();
            }
        }

        protected override void OnStop()
        {
            minerEventLog.WriteLog(0, 0, "MinerService Stopped.");
        }

        private void OnTimerWork(object sender, ElapsedEventArgs e)
        {
            minerManager.DoMining(config.XDaggerMiner?.PoolAddress, config.XDaggerMiner?.WalletAddress);

            if (namedPipeServerTask.Status != TaskStatus.Running)
            {
                namedPipeServerTask.Start();
            }
        }

        private void ConfigureMiner()
        {
            if(minerManager == null || config == null || config.Device == null)
            {
                return;
            }
            
            minerManager.ConfigureMiningDevice(config.Device.PlatformId, config.Device.DeviceId);
        }

        public void NamedPipeServerMain()
        {
            try
            {
                using (var server = new NamedPipeServerStream(string.Format(NamedPipeServerNameTemplate, config.InstanceId)))
                {
                    using (StreamReader reader = new StreamReader(server))
                    {
                        using (StreamWriter writter = new StreamWriter(server))
                        {
                            while (true)
                            {
                                server.WaitForConnection();

                                try
                                {
                                    while (true)
                                    {
                                        var line = reader.ReadLine();
                                        string output = ExecuteNamedPipeCommand(line);
                                        writter.WriteLine(output);
                                        writter.Flush();

                                        Thread.Sleep(30);
                                    }
                                }
                                catch (IOException)
                                {
                                    
                                }
                                catch (Exception)
                                {
                                    
                                }
                                finally
                                {
                                    server.Disconnect();
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
                // The NamedPipeServerStream is already opened
            }
            catch (Exception)
            {
                // TODO: Handle the exceptions
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string ExecuteNamedPipeCommand(string command)
        {
            if (command.Equals("status"))
            {
                return minerManager.QueryStatistics(1);
            }
            if (command.Equals("hashrate"))
            {
                return minerManager.QueryStatistics(2);
            }
            else
            {
                return "unknowncommand";
            }
        }
    }
}
