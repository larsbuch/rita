using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface.Exceptions;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface ISystemLogger
    {
        void logServiceStop();
        void logJobsConfigured();
        void logSchedulerStart();
        void logScheduleListCreated();
        void logTaskFactoryInitialized();
        void logServiceStart();
        void logJobRunnerEnd(string jobName, DateTime? nextRun);
        void logJobRunnerStart(string jobName);
        void logJobRunnerError(string message, Exception e);
        void logRetryRegistrationFailed(string jobName, Exception e);
        void logServiceStartupError(Exception e);
        void logRegisteringSchedulesError(Exception e);
        void logTaskTemplateWritten();
        void logScheduleLoadFailed(string scheduleName, TaskConfigurationException e);
        void logScheduleStarted(string jobName, string triggerName, DateTime? nextRun);
    }
}
