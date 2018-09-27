using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using Service = System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using XDaggerMinerDaemon.Utils;
using XDaggerMinerDaemon.Commands.Outputs;
using XDaggerMiner.Common;
using XDaggerMiner.Common.Utils;
using XDaggerMiner.Common.Contracts;
using XDaggerMinerDaemon.Services;

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

        private string serviceInstanceId = null;

        private ServiceProvider serviceProvider = null;

        private MinerConfig minerConfig = null;


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
            logger.Trace("ServiceCommand executes with parameter: " + parameter);

            minerConfig = MinerConfig.GetInstance();
            serviceProvider = ComposeServiceProvider(minerConfig.InstanceType);
            serviceInstanceId = minerConfig.InstanceId?.ToString();

            logger.Trace($"InstanceType: {minerConfig.InstanceType}. InstanceId: {serviceInstanceId}" );
            
            string operationName = parameter;
            switch (operationName)
            {
                case "start": return Start();
                case "stop": return Stop();
                case "install": return Install();
                case "uninstall": return Uninstall();
                case "detect": return Detect();
                default:
                    throw new TargetExecutionException(DaemonErrorCode.COMMAND_PARAM_ERROR, "Unknown operation type for service: " + operationName);
            }
        }

        private CommandResult Install()
        {
            try
            {
                CheckAndCleanupPreviousService();

                serviceProvider.InstallService(serviceInstanceId);
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
                serviceProvider.UninstallService(serviceInstanceId);

                // If uninstall is successful, update the config.
                if (minerConfig.UpdatedInstanceType != null)
                {
                    minerConfig.InstanceType = minerConfig.UpdatedInstanceType;
                    minerConfig.InstanceId = minerConfig.UpdatedInstanceId;
                    minerConfig.UpdatedInstanceType = null;
                    minerConfig.UpdatedInstanceId = null;

                    minerConfig.SaveToFile();
                }

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

        private CommandResult Detect()
        {
            try
            {
                string newInstanceId = serviceProvider.DetectAvailableInstanceId()?.ToString();
                return CommandResult.CreateResult(MessageOutput.Create(newInstanceId));
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

        private CommandResult Start()
        {
            logger.Trace("ServiceCommand: Start()");
            try
            {
                if (CheckAndCleanupPreviousService())
                {
                    // Reload the configs and install service first
                    serviceProvider = ComposeServiceProvider(minerConfig.InstanceType);
                    serviceInstanceId = minerConfig.InstanceId?.ToString();

                    ServiceInstance instance = serviceProvider.InstallService(serviceInstanceId);
                    instance.Start();
                }
                else
                {
                    ServiceInstance instance = serviceProvider.AquaireInstance(serviceInstanceId);
                    instance.Start();
                }

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
                ServiceInstance instance = serviceProvider.AquaireInstance(serviceInstanceId);
                instance.Stop();

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

        private bool CheckAndCleanupPreviousService()
        {
            if (minerConfig.UpdatedInstanceType == null)
            {
                // Nothing to be updated.
                logger.Trace("UpdatedInstanceType is null, no need to update.");
                return false;
            }

            // Uninstall the previous service instance
            MinerConfig.InstanceTypes instanceType = minerConfig.InstanceType ?? MinerConfig.InstanceTypes.XDagger;
            string instanceId = minerConfig.InstanceId?.ToString();

            logger.Trace($"Try to find service with InstanceType=[{instanceType}] InstanceId=[{instanceId}].");
            ServiceProvider serviceProvider = ComposeServiceProvider(minerConfig.InstanceType);
            ServiceInstance serviceInstance = serviceProvider.AquaireInstance(minerConfig.InstanceId?.ToString());
            if (serviceInstance.IsServiceExist())
            {
                logger.Trace("IsServiceExist is true, try uninstalling the previous service.");
                serviceProvider.UninstallService(instanceId);
            }

            // If the uninsallation is successful, update the config file
            minerConfig.InstanceType = minerConfig.UpdatedInstanceType;
            minerConfig.InstanceId = minerConfig.UpdatedInstanceId;
            minerConfig.UpdatedInstanceType = null;
            minerConfig.UpdatedInstanceId = null;

            minerConfig.SaveToFile();
            logger.Trace("Updated miner config file.");

            return true;
        }
    }
}
