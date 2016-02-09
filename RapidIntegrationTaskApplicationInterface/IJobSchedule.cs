using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IJobSchedule
    {
        ITaskConfiguration getTaskConfiguration();
        ITaskConfiguration getErrorTaskConfiguration();
        List<ITriggerConfiguration> getTriggerConfigurations();
        string getJobName();
        int getRetryInterval();
        int getMaxRetry();
        void configure(ITaskConfiguration taskConfiguration, ITaskConfiguration errorTaskConfiguration, string jobName, int retryInterval, int maxRetry, List<ITriggerConfiguration> triggerConfigurations);
    }
}
