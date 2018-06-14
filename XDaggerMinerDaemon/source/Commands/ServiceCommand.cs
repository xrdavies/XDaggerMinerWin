using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using Service = System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XDaggerMinerDaemon.Commands
{
    public class ServiceCommand : ConsoleCommand
    {
        private static readonly string ServiceName = @"XDaggerMinerService";
        private static readonly string ServiceBinaryName = @"XDaggerMinerService.exe";

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
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ServiceBinaryName);
            }
        }

        public override CommandInstance GenerateInstance(string[] arguments, ref int nextIndex)
        {
            if (arguments.Length <= nextIndex)
            {
                throw new ArgumentException("Argument Not Correct for Service Command.");
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
                    throw new ArgumentException("Unknown operation type for service: " + operationName);
            }
        }

        private CommandResult Install()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { ServiceBinaryFullPath });
                return CommandResult.OKResult();
            }
            catch (Exception ex)
            {
                return CommandResult.ErrorResult(101, ex.ToString());
            }
        }

        private CommandResult Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", ServiceBinaryFullPath });
                return CommandResult.OKResult();
            }
            catch (Exception ex)
            {
                return CommandResult.ErrorResult(101, ex.ToString());
            }
        }

        private CommandResult Start()
        {
            try
            {
                Service.ServiceController service = new Service.ServiceController(ServiceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

                service.Start();
                service.WaitForStatus(Service.ServiceControllerStatus.Running, timeout);

                return CommandResult.OKResult();
            }
            catch(Service.TimeoutException tex)
            {
                return CommandResult.ErrorResult(101, tex.ToString());
            }
            catch (Exception ex)
            {
                return CommandResult.ErrorResult(101, ex.ToString());
            }
        }

        private CommandResult Stop()
        {
            try
            {
                Service.ServiceController service = new Service.ServiceController(ServiceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

                service.Stop();
                service.WaitForStatus(Service.ServiceControllerStatus.Stopped, timeout);

                return CommandResult.OKResult();
            }
            catch (Service.TimeoutException tex)
            {
                return CommandResult.ErrorResult(101, tex.ToString());
            }
            catch (Exception ex)
            {
                return CommandResult.ErrorResult(101, ex.ToString());
            }
        }
    }
}
