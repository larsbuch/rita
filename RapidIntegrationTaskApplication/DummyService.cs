using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RapidIntegrationTaskApplicationExtension;
using RapidIntegrationTaskApplicationInterface;

namespace RapidIntegrationTaskApplication
{
    public class DummyService
    {
        protected IMainFactory Factory { get; set;}

        public DummyService()
        {
            Factory = MainFactory.GetMainFactory("DummyService");
        }

        public void Run()
        {
            OnStart(null);
            bool exit = false;
            while (!exit)
            {
                // Make waiting routine
                Thread.Sleep(10000);

                // Recheck if stop should be done
                if (!Factory.Scheduler.IsStarted)
                {
                    exit = true;
                }
            }
            OnStop();
        }

        protected void OnStart(string[] args)
        {
            Factory.loadAndConfigureFactory();
        }

        protected void OnStop()
        {
            Factory.Dispose();
        }
    }
}
