using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobSchedule:IJobSchedule
    {
        private ITaskConfiguration _taskConfiguration;
        private ITaskConfiguration _errorTaskConfiguration;
        private string _jobName;
        private int _retryInterval;
        private int _maxRetry;
        private List<ITriggerConfiguration> _triggerConfigurations;
        
        public ITaskConfiguration getTaskConfiguration()
        {
            return _taskConfiguration;
        }

        public ITaskConfiguration getErrorTaskConfiguration()
        {
            return _errorTaskConfiguration;
        }

        public List<ITriggerConfiguration> getTriggerConfigurations()
        {
            return _triggerConfigurations;
        }

        public string getJobName()
        {
            return _jobName;
        }

        public int getRetryInterval()
        {
            return _retryInterval;
        }

        public int getMaxRetry()
        {
            return _maxRetry;
        }

        public void configure(ITaskConfiguration taskConfiguration, ITaskConfiguration errorTaskConfiguration, string jobName, int retryInterval, int maxRetry, List<ITriggerConfiguration> triggerConfigurations)
        {
            _taskConfiguration = taskConfiguration;
            _errorTaskConfiguration = errorTaskConfiguration;
            _jobName = jobName;
            _retryInterval = retryInterval;
            _maxRetry = maxRetry;
            _triggerConfigurations = triggerConfigurations;
        }
    }
}
