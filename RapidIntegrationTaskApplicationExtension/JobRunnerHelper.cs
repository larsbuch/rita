using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using Quartz;

namespace RukisIntegrationTaskhandlerExtension
{
    public static class JobRunnerHelper
    {
        public static void registerRetryJob(IMainFactory mainFactory, int retryInterval, JobDetail originalJobDetail)
        {
            try
            {
                JobDetail jobDetail = mainFactory.JobRunnerFactory.getNewRetryJobRunner(originalJobDetail);

                // Create Trigger for running once
                Trigger trigger = mainFactory.JobTriggerFactory.getRetryTrigger(jobDetail.Name, retryInterval);
                mainFactory.Scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (JobRunnerException e)
            {
                mainFactory.SystemLogger.logRetryRegistrationFailed(
                    originalJobDetail.Name, e);
            }
        }

        #region Execution

        public static ITaskConfiguration getTaskConfiguration(JobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object taskList = jobDataMap[Context.TaskConfiguration];
            if (taskList is ITaskConfiguration)
            {
                ITaskConfiguration taskConfiguration = taskList as ITaskConfiguration;
                taskConfiguration.setJobScheduleName(jobExecutionContext.JobDetail.Name);
                return taskConfiguration;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Name,
                    "getTaskConfiguration",
                    string.Format("Error in context as {0} is not ITaskConfiguration", Context.TaskConfiguration));
            }
        }

        public static ITaskConfiguration getErrorTaskConfiguration(JobExecutionContext jobExecutionContext)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            object taskList = jobDataMap[Context.ErrorTaskConfiguration];
            if (taskList is ITaskConfiguration)
            {
                ITaskConfiguration taskConfiguration = taskList as ITaskConfiguration;
                taskConfiguration.setJobScheduleName(jobExecutionContext.JobDetail.Name);
                return taskConfiguration;
            }
            else
            {
                throw new JobRunnerException(
                    jobExecutionContext.JobDetail.Name,
                    "getErrorTaskConfiguration",
                    string.Format("Error in context as {0} is not ITaskConfiguration", Context.ErrorTaskConfiguration));
            }
        }

        public static string getJobName(JobExecutionContext jobExecutionContext)
        {
            return jobExecutionContext.JobDetail.Name;
        }

        public static bool jobCanRetry(JobExecutionContext jobExecutionContext)
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
                    jobExecutionContext.JobDetail.Name,
                    "jobCanRetry",
                    string.Format("Error in context as {0} or {1} is not an int",
                                  Context.CurrentRetry, Context.MaxRetry));
            }
        }

        public static bool getIsRecovering(JobExecutionContext jobExecutionContext)
        {
            return jobExecutionContext.Recovering;
        }

        public static int getCurrentRetry(JobExecutionContext jobExecutionContext)
        {
            return getCurrentRetry(jobExecutionContext.JobDetail);
        }

        public static string getCurrentTaskName(JobExecutionContext jobExecutionContext)
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
                    jobExecutionContext.JobDetail.Name,
                    "getCurrentTaskName",
                    string.Format("Error in context as {0} is not string",
                                  Context.CurrentTaskName));
            }
        }

        public static void setCurrentTaskName(JobExecutionContext jobExecutionContext, string executingTaskName)
        {
            JobDataMap jobDataMap = jobExecutionContext.JobDetail.JobDataMap;
            jobDataMap[Context.CurrentTaskName] = executingTaskName;
        }

        public static int getRetryInterval(JobExecutionContext jobExecutionContext)
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
                    jobExecutionContext.JobDetail.Name,
                    "getCurrentTaskName",
                    string.Format("Error in context as {0} is not an int",
                                  Context.RetryInterval));
            }
        }

        #endregion

        #region Configuration

        public static int getCurrentRetry(JobDetail jobDetail)
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
                    jobDetail.Name,
                    "getCurrentRetry",
                    string.Format("Error in context as {0} is not an int",
                                  Context.CurrentRetry));
            }
        }

        public static void setContext(JobDetail jobDetail, ITaskConfiguration taskConfiguration,
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

            // Retry i case of crash while running
            jobDetail.RequestsRecovery = true;

            // Forget job if no trigger is associated with it
            jobDetail.Durable = false;
        }

        private static void increaseRetry(JobDetail jobDetail, JobDetail originalJobDetail)
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
                    jobDetail.Name,
                    "increaseRetry",
                    string.Format("Error in context as {0} is not an int",
                                  Context.CurrentRetry));
            }
            jobDetail.JobDataMap[Context.CurrentRetry] = newCurrentRetry;
        }

        public static void makeRetryContext(string factoryName,JobDetail jobDetail, JobDetail originalJobDetail)
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

            // Set retry version higher
            increaseRetry(jobDetail, originalJobDetail);

            // Forget job if no trigger is associated with it
            jobDetail.Durable = false;
        }

        #endregion
    }
}