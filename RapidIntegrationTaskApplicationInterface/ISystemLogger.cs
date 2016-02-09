using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ISystemLogger
    {
        void logServiceStop();
        void logJobsConfigured();
        void logSchedulerStart();
        void logScheduleListCreated();
        void logTaskFactoryInitialized();
        void logServiceStart();
        void logJobRunnerEnd(string jobName, DateTimeOffset? nextRun);
        void logJobRunnerStart(string jobName);
        void logJobRunnerError(string message, Exception e);
        void logRetryRegistrationFailed(string jobName, Exception e);
        void logServiceStartupError(Exception e);
        void logRegisteringSchedulesError(Exception e);
        void logTaskTemplateWritten();
        void logScheduleLoadFailed(string scheduleName, TaskConfigurationException e);
        void logScheduleStarted(string jobName, string triggerName, DateTimeOffset? nextRun);
    }
}
