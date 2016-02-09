using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public static class JobRunnerHelper
    {
        public static void registerRetryJob(IMainFactory mainFactory, int retryInterval, IJobDetail originalJobDetail)
        {
            try
            {
                IJobDetail jobDetail = mainFactory.JobRunnerFactory.getNewRetryJobRunner(originalJobDetail);

                // Create Trigger for running once
                ITrigger trigger = mainFactory.JobTriggerFactory.getRetryTrigger(jobDetail.Key.Name, retryInterval);
                mainFactory.Scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (JobRunnerException e)
            {
                mainFactory.SystemLogger.logRetryRegistrationFailed(
                    originalJobDetail.Key.Name, e);
            }
        }

        #region Execution

        public static ITaskConfiguration getTaskConfiguration(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object taskList = jobDataMap[Context.TaskConfiguration];
            if (taskList is ITaskConfiguration)
            {
                ITaskConfiguration taskConfiguration = taskList as ITaskConfiguration;
                taskConfiguration.setJobScheduleName(jobExecutionContext.JobDetail.Key.Name);
                return taskConfiguration;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "getTaskConfiguration",
                    string.Format("Error in context as {0} is not ITaskConfiguration", Context.TaskConfiguration));
            }
        }

        public static ITaskConfiguration getErrorTaskConfiguration(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object taskList = jobDataMap[Context.ErrorTaskConfiguration];
            if (taskList is ITaskConfiguration)
            {
                ITaskConfiguration taskConfiguration = taskList as ITaskConfiguration;
                taskConfiguration.setJobScheduleName(jobExecutionContext.JobDetail.Key.Name);
                return taskConfiguration;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "getErrorTaskConfiguration",
                    string.Format("Error in context as {0} is not ITaskConfiguration", Context.ErrorTaskConfiguration));
            }
        }

        public static string getJobName(IJobExecutionContext jobExecutionContext)
        {
            return jobExecutionContext.JobDetail.Key.Name;
        }

        public static bool jobCanRetry(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object currentRetry = jobDataMap[Context.CurrentRetry];
            object maxRetry = jobDataMap[Context.MaxRetry];
            if (currentRetry is int && maxRetry is int)
            {
                int castCurrentRetry = (int) currentRetry;
                int castMaxRetry = (int) maxRetry;

                if (castCurrentRetry <= castMaxRetry)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "jobCanRetry",
                    string.Format("Error in context as {0} or {1} is not an int",
                                  Context.CurrentRetry, Context.MaxRetry));
            }
        }

        public static bool getIsRecovering(IJobExecutionContext jobExecutionContext)
        {
            return jobExecutionContext.Recovering;
        }

        public static int getCurrentRetry(IJobExecutionContext jobExecutionContext)
        {
            return getCurrentRetry(jobExecutionContext.JobDetail);
        }

        public static string getCurrentTaskName(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object taskName = jobDataMap[Context.CurrentTaskName];
            if (taskName is string)
            {
                return taskName as string;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "getCurrentTaskName",
                    string.Format("Error in context as {0} is not string",
                                  Context.CurrentTaskName));
            }
        }

        public static string getLifetimeName(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object lifetimeName = jobDataMap[Context.LifetimeName];
            if (lifetimeName is string)
            {
                return lifetimeName as string;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "getLifetimeName",
                    string.Format("Error in context as {0} is not string",
                                  Context.CurrentTaskName));
            }
        }

        public static void setCurrentTaskName(IJobExecutionContext jobExecutionContext, string executingTaskName)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            jobDataMap[Context.CurrentTaskName] = executingTaskName;
        }

        public static int getRetryInterval(IJobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object retryInterval = jobDataMap[Context.RetryInterval];
            if (retryInterval is int)
            {
                return (int) retryInterval;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Key.Name,
                    "getCurrentTaskName",
                    string.Format("Error in context as {0} is not an int",
                                  Context.RetryInterval));
            }
        }

        #endregion

        #region Configuration

        public static int getCurrentRetry(IJobDetail jobDetail)
        {
            JobDataMap jobDataMap = jobDetail.JobDataMap;
            object currentRetry = jobDataMap[Context.CurrentRetry];
            if (currentRetry is int)
            {
                return (int) currentRetry;
            }
            else
            {
                throw new JobRunnerException(
                    jobDetail.Key.Name,
                    "getCurrentRetry",
                    string.Format("Error in context as {0} is not an int",
                                  Context.CurrentRetry));
            }
        }

        public static void setContext(string lifetimeName, IJobDetail jobDetail, ITaskConfiguration taskConfiguration,
                                      ITaskConfiguration errorTaskConfiguration,
                                      int retryInterval, int maxRetry)
        {
            JobDataMap jobDataMap = jobDetail.JobDataMap;
            jobDataMap[Context.TaskConfiguration] = taskConfiguration;
            jobDataMap[Context.ErrorTaskConfiguration] = errorTaskConfiguration;
            jobDataMap[Context.RetryInterval] = retryInterval;
            jobDataMap[Context.MaxRetry] = maxRetry;
            jobDataMap[Context.CurrentRetry] = 0;
            jobDataMap[Context.CurrentTaskName] = Task.StartFromFirstTask;
            jobDataMap[Context.LifetimeName] = lifetimeName;

            // Retry i case of crash while running
            // TODO jobDetail.RequestsRecovery = true;

            // Forget job if no trigger is associated with it
            // TODO jobDetail.Durable = false;
        }

        private static void increaseRetry(IJobDetail jobDetail, IJobDetail originalJobDetail)
        {
            JobDataMap jobDataMap = originalJobDetail.JobDataMap;
            object currentRetry = jobDataMap[Context.CurrentRetry];
            int newCurrentRetry;
            if (currentRetry is int)
            {
                newCurrentRetry = ((int) currentRetry) + 1;
            }
            else
            {
                throw new JobRunnerException(
                    jobDetail.Key.Name,
                    "increaseRetry",
                    string.Format("Error in context as {0} is not an int",
                                  Context.CurrentRetry));
            }
            jobDetail.JobDataMap[Context.CurrentRetry] = newCurrentRetry;
        }

        public static void makeRetryContext(string factoryName,IJobDetail jobDetail, IJobDetail originalJobDetail)
        {
            if (jobDetail == null || originalJobDetail == null || originalJobDetail.JobDataMap == null ||
                jobDetail.JobDataMap == null ||
                originalJobDetail.JobDataMap[Context.TaskConfiguration] == null ||
                !(originalJobDetail.JobDataMap[Context.TaskConfiguration] is ITaskConfiguration) ||
                originalJobDetail.JobDataMap[Context.ErrorTaskConfiguration] == null ||
                !(originalJobDetail.JobDataMap[Context.ErrorTaskConfiguration] is ITaskConfiguration))
            {
                throw new FactoryException(factoryName, "makeRetryContext",
                                             "parameters jobDetail or originalJobDetail does not have expected values");
            }

            ITaskConfiguration taskConfiguration =
                originalJobDetail.JobDataMap[Context.TaskConfiguration] as ITaskConfiguration;

            jobDetail.JobDataMap[Context.TaskConfiguration] = taskConfiguration.getCopy(null, true);
            ITaskConfiguration errorTaskConfiguration = originalJobDetail.JobDataMap[Context.ErrorTaskConfiguration] as ITaskConfiguration;
            jobDetail.JobDataMap[Context.ErrorTaskConfiguration] =
                errorTaskConfiguration.getCopy(null, true);
            jobDetail.JobDataMap[Context.RetryInterval] = originalJobDetail.JobDataMap[Context.RetryInterval];
            jobDetail.JobDataMap[Context.MaxRetry] = originalJobDetail.JobDataMap[Context.MaxRetry];
            jobDetail.JobDataMap[Context.CurrentTaskName] = originalJobDetail.JobDataMap[Context.CurrentTaskName];
            jobDetail.JobDataMap[Context.LifetimeName] = originalJobDetail.JobDataMap[Context.LifetimeName];

            // Set retry version higher
            increaseRetry(jobDetail, originalJobDetail);

            // Forget job if no trigger is associated with it
            // TODO jobDetail.Durable = false;
        }

        #endregion
    }
}