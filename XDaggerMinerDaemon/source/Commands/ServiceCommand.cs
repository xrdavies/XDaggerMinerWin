﻿using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using Service = System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerDaemon.Utils;
using XDaggerMiner.Common;

namespace XDaggerMinerDaemon.Commands
{
    public class ServiceCommand : ConsoleCommand
    {
        public override string GetShortName()
        {
            return "-s";
        }

        public override string GetLongName()
        {
            return "--Service";
        }

        private static string ServiceBinaryFullPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ServiceUtil.ServiceBinaryName);
            }
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            if (arguments.Length <= nextIndex)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "Argument Not Correct for Service Command.");
            }

            CommandInstance instance = new CommandInstance(this);
            instance.Parameter = arguments[nextIndex++]?.Trim().ToLower();

            return instance;
        }

        public override CommandResult Execute(string parameter)
        {
            string operationName = parameter;
            switch (operationName)
            {
                case "start": return Start();
                case "stop": return Stop();
                case "install": return Install();
                case "uninstall": return Uninstall();
                default:
                    throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "Unknown operation type for service: " + operationName);
            }
        }

        private CommandResult Install()
        {
            try
            {
                ServiceUtil.InstallService(ServiceBinaryFullPath);
                return CommandResult.OKResult();
            }
            catch (TargetExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }

        private CommandResult Uninstall()
        {
            try
            {
                ServiceUtil.UninstallService(ServiceBinaryFullPath);
                return CommandResult.OKResult();
            }
            catch (TargetExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }

        private CommandResult Start()
        {
            try
            {
                ServiceUtil.StartService(ServiceUtil.ServiceName);

                return CommandResult.OKResult();
            }
            catch(Service.TimeoutException ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_TIME_OUT, ex);
            }
            catch (TargetExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }

        private CommandResult Stop()
        {
            try
            {
                ServiceUtil.StopService(ServiceUtil.ServiceName);
                return CommandResult.OKResult();
            }
            catch (Service.TimeoutException ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.COMMAND_TIME_OUT, ex);
            }
            catch (TargetExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new TargetExecutionException(DaemonErrorCode.UNKNOWN_ERROR, ex);
            }
        }
    }
}
