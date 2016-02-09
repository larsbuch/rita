using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using RapidIntegrationTaskApplicationExtension;
using RapidIntegrationTaskApplicationInterface;
using Common.Logging;
using Quartz;

namespace RapidIntegrationTaskApplication
{
    public partial class RapidIntegrationTaskApplicationService : ServiceBase
    {
        protected IMainFactory Factory
        {
            get
            {
                return MainFactory.GetMainFactory("MainService");
            }
        }

        public RapidIntegrationTaskApplicationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Factory.loadAndConfigureFactory();
        }

        protected override void OnStop()
        {
            Factory.Dispose();
        }
    }
}
