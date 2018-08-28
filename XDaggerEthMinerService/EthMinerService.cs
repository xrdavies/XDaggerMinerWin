﻿using System;
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
using log4net;
using XDaggerMiner.Common;
using XDaggerMiner.Common.Contracts;

namespace XDaggerEthMinerService
{
    public enum MinerState
    {

    }

    public partial class EthMinerService : ServiceBase
    {
        private static readonly string NamedPipeServerNameTemplate = "XDaggerEthMinerPipe_{0}";

        private MinerConfig minerConfig = null;

        private Process ehtMinerProcess = null;

        private EventLog implementEventLog = null;

        private ILog logger = LogManager.GetLogger("EthMinerService");

        private static TimeSpan timerWorkInterval = TimeSpan.FromSeconds(5);

        private bool isTimerWorkRunning = false;

        private Task namedPipeServerTask = null;

        public EthMinerService()
        {
            InitializeComponent();

            minerConfig = MinerConfig.ReadFromFile();

            namedPipeServerTask = new Task(() =>
            {
                NamedPipeServerMain();
            });
        }

        protected override void OnStart(string[] args)
        {
            InitializeEventLog();

            LaunchEthMiner();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = timerWorkInterval.TotalMilliseconds;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimerWork);
            timer.Start();

            if (namedPipeServerTask.Status != TaskStatus.Running)
            {
                namedPipeServerTask.Start();
            }

        }

        private void OnTimerWork(object sender, ElapsedEventArgs e)
        {
            if (isTimerWorkRunning)
            {
                return;
            }

            isTimerWorkRunning = true;
            logger.Debug("OnTimerWork");
            this.implementEventLog.WriteEntry("OnTimerWork");

            DateTime timerWorkStartTime = DateTime.UtcNow;

            StreamReader reader = ehtMinerProcess.StandardError;
            while (reader != null && !reader.EndOfStream && timerWorkStartTime.Add(timerWorkInterval) > DateTime.UtcNow)
            {
                string line = reader.ReadLine();
                logger.Debug(line);
                //// this.implementEventLog.WriteEntry(line);

                Thread.Sleep(30);
            }

            isTimerWorkRunning = false;
        }

        private void InitializeEventLog()
        {
            string sourceName = "XDaggerEthMiner";
            string logName = "XDaggerMinerLog";

            this.implementEventLog = new EventLog();
            ((ISupportInitialize)(this.implementEventLog)).BeginInit();
            ((ISupportInitialize)(this.implementEventLog)).EndInit();

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(
                    sourceName, logName);
            }

            this.implementEventLog.Source = sourceName;
            this.implementEventLog.Log = logName;
        }

        private void LaunchEthMiner()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);

            string ethExecutionFileFullPath = Path.Combine(directoryPath, "External/ethminer.exe");

            ehtMinerProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            startInfo.FileName = ethExecutionFileFullPath;
            startInfo.Arguments = "-G " + minerConfig.EthMiner?.PoolAddress;
            ehtMinerProcess.StartInfo = startInfo;

            try
            {
                ehtMinerProcess.Start();
                isTimerWorkRunning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.implementEventLog.WriteEntry(e.Data);

        }

        protected override void OnStop()
        {
            if (ehtMinerProcess != null && !ehtMinerProcess.HasExited)
            {
                try
                {
                    ehtMinerProcess.Kill();
                }
                catch(Exception)
                {
                    // Swallow the exceptions
                }
            }
        }

        public void NamedPipeServerMain()
        {
            try
            {
                using (var server = new NamedPipeServerStream(string.Format(NamedPipeServerNameTemplate, 0)))
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
                return MinerServiceState.Mining;
            }
            if (command.Equals("hashrate"))
            {
                return "0";
            }
            else
            {
                return "unknowncommand";
            }
        }
    }
}
