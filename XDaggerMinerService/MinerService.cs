using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XDaggerMinerDaemon;
using XDaggerMinerRuntimeCLI;

namespace XDaggerMinerService
{
    public partial class MinerService : ServiceBase
    {
        private static readonly string NamedPipeServerNameTemplate = "XDaggerMinerPipe_{0}";

        private MinerEventLog minerEventLog;

        private MinerManager minerManager;

        private MinerConfig config;

        private string instanceId = null;

        private Task pipelineServerTask = null;

        public MinerService()
        {
            InitializeComponent();

            config = MinerConfig.ReadFromFile();

            minerEventLog = new MinerEventLog();
            minerManager = new MinerManager(config.IsFakeRun);
            minerManager.SetLogger(minerEventLog);

            pipelineServerTask = new Task(() =>
            {
                NamedPipeServerMain();
            });
        }

        protected override void OnStart(string[] args)
        {
            
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerWork);
            timer.Start();

            if (pipelineServerTask.Status != TaskStatus.Running)
            {
                pipelineServerTask.Start();
            }

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

            if (pipelineServerTask.Status != TaskStatus.Running)
            {
                pipelineServerTask.Start();
            }
        }

        public void NamedPipeServerMain()
        {
            try
            {
                using (var server = new NamedPipeServerStream(string.Format(NamedPipeServerNameTemplate, instanceId)))
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
            catch (Exception ex)
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
                return "running";
            }
            if (command.Equals("hashrate"))
            {
                return "15.4";
            }
            else
            {
                return "unknown";
            }
        }
    }
}
