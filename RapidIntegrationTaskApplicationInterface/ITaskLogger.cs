using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ITaskLogger
    {
        void logJobRunnerStart(string jobName);
        void logTaskStart(string taskName);
        void logTaskEnd(string taskName);
        void logJobRunnerEnd(string jobName, DateTimeOffset? nextTimeFiring);
        void logErrorTaskStart(string taskName);
        void logErrorTaskEnd(string taskName);
        void logTaskResetStart(string taskName);
        void logTaskResetEnd(string taskName);
        void logTaskResetFailed(string taskName);
        void logRetryRegistering(string jobName, int retryInterval);
        void logRetryRegistered(string jobName);
        void logJobRunnerConfiguring(string jobName);
        void logTaskNameSet(string jobName, string taskName);
        void logTaskExecutionException(string jobName, string taskName, TaskExecutionException e);
        void logDebugText(string s);
    }
}
