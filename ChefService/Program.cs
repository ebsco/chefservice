using System.ServiceProcess;

namespace ChefService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ChefService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
