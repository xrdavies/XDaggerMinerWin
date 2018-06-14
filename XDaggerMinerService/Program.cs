using System.ServiceProcess;

namespace XDaggerMinerService
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
               ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MinerService()
                };
                ServiceBase.Run(ServicesToRun);
            
        }
    }
}
