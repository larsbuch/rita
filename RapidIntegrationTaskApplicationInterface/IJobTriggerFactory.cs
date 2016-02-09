using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Quartz;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IJobTriggerFactory
    {
        IMainFactory MainFactory { get; set; }
        List<ITrigger> getJobTriggers(List<ITriggerConfiguration> triggerConfigurations);
        ITrigger getRetryTrigger(string jobName, int retryInterval);
        List<ITriggerConfiguration> buildTriggerConfigurationList(IJobSchedule jobSchedule,XmlNode xmlNode);
    }
}
