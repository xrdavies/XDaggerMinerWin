using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;
using XDaggerMinerDaemon.Commands.Outputs;
using XDaggerMinerDaemon.Utils;

namespace XDaggerMinerDaemon.Commands
{
    public class ReportCommand : ConsoleCommand
    {
        private static readonly string NamedPipeServerNameTemplate = "XDaggerMinerPipe_{0}";
        
        public override string GetShortName()
        {
            return "-r";
        }

        public override string GetLongName()
        {
            return "--Report";
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            CommandInstance instance = new CommandInstance(this);

            return instance;
        }

        public override CommandResult Execute(string parameter)
        {
            try
            {
                ReportOutput outputResult = new ReportOutput();
                outputResult.Status = ReportOutput.StatusEnum.Unknown;

                if (!ServiceUtil.CheckServiceExist(ServiceUtil.ServiceName))
                {
                    outputResult.Status = ReportOutput.StatusEnum.NotInstalled;
                }
                else
                {
                    if (!ServiceUtil.IsServiceRunning(ServiceUtil.ServiceName))
                    {
                        outputResult.Status = ReportOutput.StatusEnum.Stopped;
                    }
                    else
                    {
                        // Retrieve the miner status from NamedPipe
                        QueryServiceStatusByNamedPipe(outputResult);
                    }
                }

                return CommandResult.CreateResult(outputResult);
            }
            catch(TargetExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }

        /// <summary>
        /// Sample return value from Service: 
        ///     status      : running|disconnected|connected
        ///     hash_rate   : [double]
        /// </summary>
        /// <param name="hashRate"></param>
        /// <returns></returns>
        private void QueryServiceStatusByNamedPipe(ReportOutput outputResult)
        {
            string namedPipeName = string.Format(NamedPipeServerNameTemplate, string.Empty);
            outputResult.HashRate = 0;

            string pipelineOutput = string.Empty;
            try
            {
                using (var client = new NamedPipeClientStream(namedPipeName))
                {
                    client.Connect(3000);

                    using (StreamReader reader = new StreamReader(client))
                    {
                        using (StreamWriter writer = new StreamWriter(client))
                        {
                            string status = ReadFromNamedPipe(reader, writer, "status");
                            string hashRateStr = ReadFromNamedPipe(reader, writer, "hashrate");
                            switch(status)
                            {
                                case "running":
                                    outputResult.Status = ReportOutput.StatusEnum.Mining;
                                    break;
                                case "connected":
                                    outputResult.Status = ReportOutput.StatusEnum.Connected;
                                    break;
                                case "disconnected":
                                    outputResult.Status = ReportOutput.StatusEnum.Disconnected;
                                    break;
                                default:
                                    break;
                            }

                            outputResult.HashRate = Double.Parse(hashRateStr);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //TODO: Handle exceptions
                throw new TargetExecutionException(DaemonErrorCode.REPORT_NAMEDPIPE_ERROR, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReadFromNamedPipe(StreamReader reader, StreamWriter writer, string input)
        {
            writer.WriteLine(input);
            writer.Flush();
            return reader.ReadLine();
        }
    }
}
