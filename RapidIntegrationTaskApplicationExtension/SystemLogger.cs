using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Common.Logging;

namespace RapidIntegrationTaskApplicationExtension
{
    public class SystemLogger : ISystemLogger
    {
        private ILog _logger;

        private ILog InnerLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(Logger.SystemLogger);
                }
                return _logger;
            }
        }

        public void logServiceStop()
        {
            InnerLogger.Info("Service stopped");
        }

        public void logJobsConfigured()
        {
            InnerLogger.Debug("Jobs has beed configured");
        }

        public void logSchedulerStart()
        {
            InnerLogger.Debug("Scheduler has started");
        }

        public void logScheduleListCreated()
        {
            InnerLogger.Debug("Schedule list created");
        }

        public void logTaskFactoryInitialized()
        {
            InnerLogger.Debug("Task Factory initialized");
        }

        public void logServiceStart()
        {
            InnerLogger.Info("Service started");
        }

        public void logJobRunnerEnd(string jobName, DateTimeOffset? nextRun)
        {
            InnerLogger.Info(string.Format("JobRunner ending with job {0}. Next run at {1}", jobName, nextRun));
        }

        public void logJobRunnerStart(string jobName)
        {
            InnerLogger.Info(string.Format("JobRunner starting with job {0}",jobName));
        }

        public void logJobRunnerError(string message, Exception e)
        {
            InnerLogger.Error(message, e);
        }

        public void logRetryRegistrationFailed(string jobName, Exception e)
        {
            InnerLogger.Error(string.Format("Registration of retry of {0} failed", jobName),e);
        }

        public void logServiceStartupError(Exception e)
        {
            InnerLogger.Error("The system failed to startup",e);
        }

        public void logRegisteringSchedulesError(Exception e)
        {
            InnerLogger.Error("Error occured while registering schedules", e);
        }

        public void logTaskTemplateWritten()
        {
            InnerLogger.Debug("Task templates has been written");
        }

        public void logScheduleLoadFailed(string scheduleName, TaskConfigurationException e)
        {
            InnerLogger.Error(string.Format("There is an error in the schedule {0}. Schedule has not been loaded and will not be executed. \n{1}", scheduleName,e.Message));
        }

        public void logScheduleStarted(string jobName, string triggerName, DateTimeOffset? nextRun)
        {
            InnerLogger.Debug(string.Format("Job {0} with trigger {1} running next time at {2}", jobName,triggerName,nextRun));
        }
    }
}