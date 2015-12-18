using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Quartz;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface IJobTriggerFactory
    {
        IMainFactory MainFactory { get; set; }
        List<Trigger> getJobTriggers(List<ITriggerConfiguration> triggerConfigurations);
        Trigger getRetryTrigger(string jobName, int retryInterval);
        List<ITriggerConfiguration> buildTriggerConfigurationList(IJobSchedule jobSchedule,XmlNode xmlNode);
    }
}
