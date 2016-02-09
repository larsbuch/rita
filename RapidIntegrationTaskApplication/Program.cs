using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RapidIntegrationTaskApplication
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
#if DEBUG
            DummyService dummyService = new DummyService();
            dummyService.Run();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
                                {
                                    new RapidIntegrationTaskApplicationService()
                                };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}