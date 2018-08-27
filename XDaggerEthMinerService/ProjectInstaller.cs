using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace XDaggerEthMinerService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            string instanceId = Context.Parameters["instance"];

            if (!string.IsNullOrEmpty(instanceId))
            {
                this.serviceInstaller.DisplayName = string.Format(ServiceDisplayNameByInstance, instanceId);
                this.serviceInstaller.ServiceName = string.Format(ServiceServiceNameByInstance, instanceId);
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            string instanceId = Context.Parameters["instance"];

            if (!string.IsNullOrEmpty(instanceId))
            {
                this.serviceInstaller.DisplayName = string.Format(ServiceDisplayNameByInstance, instanceId);
                this.serviceInstaller.ServiceName = string.Format(ServiceServiceNameByInstance, instanceId);
            }
        }

        private void serviceInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            string instanceId = Context.Parameters["instance"];

            if (!string.IsNullOrEmpty(instanceId))
            {
                this.serviceInstaller.DisplayName = string.Format(ServiceDisplayNameByInstance, instanceId);
                this.serviceInstaller.ServiceName = string.Format(ServiceServiceNameByInstance, instanceId);
            }
        }
    }
}
