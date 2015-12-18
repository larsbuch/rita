using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using RukisIntegrationTaskhandlerExtension;
using RukisIntegrationTaskhandlerInterface;
using Common.Logging;
using Quartz;

namespace RukisIntegrationTaskhandler
{
    public partial class RukisIntegrationTaskhandlerService : ServiceBase
    {
        protected IMainFactory Factory { get { return MainFactory.Current; } }

        public RukisIntegrationTaskhandlerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Factory.loadAndConfigureFactory();
        }

        protected override void OnStop()
        {
            Factory.unloadFactory();
        }
    }
}
