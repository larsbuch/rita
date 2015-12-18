using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using Common.Logging;

namespace RukisIntegrationTaskhandlerExtension
{
    public class TaskLogger:ITaskLogger
    {
        private ILog _logger;

        private ILog InnerLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(Logger.TaskLogger);
                }
                return _logger;
            }
        }

        public void logJobRunnerStart(string jobName)
        {
            InnerLogger.Info(string.Format("Starting job {0}", jobName));
        }

        public void logTaskStart(string taskName)
        {
            InnerLogger.Debug(string.Format("Starting task {0}", taskName));
        }

        public void logTaskEnd(string taskName)
        {
            InnerLogger.Debug(string.Format("Ending task {0}", taskName));
        }

        public void logJobRunnerEnd(string jobName, DateTime? nextRun)
        {
            InnerLogger.Info(string.Format("Ending job {0}. Next run at {1}.", jobName, nextRun));
        }

        public void logErrorTaskStart(string taskName)
        {
            InnerLogger.Info(string.Format("Error task {0} starts executing", taskName));
        }

        public void logErrorTaskEnd(string taskName)
        {
            InnerLogger.Info(string.Format("Error task {0} starts executing", taskName));
        }

        public void logTaskResetStart(string taskName)
        {
            InnerLogger.Info(string.Format("Task {0} starts to reset", taskName));
        }

        public void logTaskResetEnd(string taskName)
        {
            InnerLogger.Info(string.Format("Task {0} has ended reset", taskName));
        }

        public void logTaskResetFailed(string taskName)
        {
            InnerLogger.Error(string.Format("Could not reset failed task {0}",taskName));
        }

        public void logRetryRegistering(string jobName, int retryInterval)
        {
            InnerLogger.Info(string.Format("Retry registering for {0} in {1} minutes",jobName,retryInterval));
        }

        public void logRetryRegistered(string jobName)
        {
            InnerLogger.Info(string.Format("Retry registered for {0}", jobName));
        }

        public void logJobRunnerConfiguring(string jobName)
        {
            InnerLogger.Debug(string.Format("JobRunner configured for {0}",jobName));
        }

        public void logTaskNameSet(string jobName, string taskName)
        {
            InnerLogger.Debug(string.Format("Execution task name {0} in job {1} is set", taskName, jobName));
        }

        public void logTaskExecutionException(string jobName, string taskName, TaskExecutionException e)
        {
            InnerLogger.Error(string.Format("Task {1} execution threw an exception in job {0}", jobName, taskName), e);
        }

        public void logDebugText(string s)
        {
            InnerLogger.Debug(s);
        }

        public void logJobRunnerRestart(string jobName)
        {
            InnerLogger.Info(string.Format("Restarting job {0}", jobName));
        }
    }
}
