namespace XDaggerEthMinerService
{
    partial class ProjectInstaller
    {
        private static readonly string DefaultServiceDisplayName = @"XDagger Eth Miner Service";
        private static readonly string DefaultServiceServiceName = @"XDaggerEthMinerService";

        private static readonly string ServiceDisplayNameByInstance = @"XDagger Eth Miner Service {0}";
        private static readonly string ServiceServiceNameByInstance = @"XDaggerEthMinerService_{0}";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstaller
            // 
            /*
            string instanceId = Context.Parameters["instance"];
            if (!string.IsNullOrEmpty(instanceId))
            {
                this.serviceInstaller.DisplayName = string.Format(ServiceDisplayNameByInstance, instanceId);
                this.serviceInstaller.ServiceName = string.Format(ServiceServiceNameByInstance, instanceId);
            }
            else
            {
                this.serviceInstaller.DisplayName = DefaultServiceDisplayName;
                this.serviceInstaller.ServiceName = DefaultServiceServiceName;
            }
            */

            this.serviceInstaller.DisplayName = DefaultServiceDisplayName;
            this.serviceInstaller.ServiceName = DefaultServiceServiceName;
            this.serviceInstaller.Description = "Service for XDagger Wrapped Eth Miner Pool";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            //// this.serviceInstaller.BeforeInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller_BeforeInstall);

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
    }
}