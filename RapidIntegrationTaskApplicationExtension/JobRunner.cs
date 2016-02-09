using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Common.Logging;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobRunner:IJobRunner
    {
        private ITaskLogger _taskLogger = null;
        private ISystemLogger _systemLogger = null;
        private IJobExecutionContext _jobExecutionContext = null;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // Set initial values
                _jobExecutionContext = context;
                IMainFactory mainFactory = MainFactory.GetMainFactory(LifetimeName);
                _taskLogger = mainFactory.TaskLogger;
                _systemLogger = mainFactory.SystemLogger;

                string jobName = JobRunnerHelper.getJobName(context);

                TaskLogger.logJobRunnerConfiguring(jobName);

                ITaskConfiguration taskConfiguration = JobRunnerHelper.getTaskConfiguration(context);
                ITaskConfiguration errorTaskConfiguration = JobRunnerHelper.getErrorTaskConfiguration(context);
                int retryInterval = JobRunnerHelper.getRetryInterval(context);
                string startAtTask = JobRunnerHelper.getCurrentTaskName(context);

                ITaskExecutionContext taskExecutionContext = new TaskExecutionContext(this, startAtTask);

                ITask task = mainFactory.TaskFactory.buildTask(this,taskConfiguration);
                ITask errorTask = mainFactory.TaskFactory.buildTask(this,errorTaskConfiguration);

                TaskLogger.logJobRunnerStart(jobName);
                SystemLogger.logJobRunnerStart(jobName);
                try
                {
                    try
                    {
                        task.executeTask(taskExecutionContext);
                        CurrentTaskName = Task.StartFromFirstTask;
                    }
                    catch (TaskExecutionException e)
                    {
                        TaskLogger.logTaskExecutionException(jobName, JobRunnerHelper.getCurrentTaskName(context), e);

                        if (JobRunnerHelper.jobCanRetry(context))
                        {
                            TaskLogger.logRetryRegistering(context.JobDetail.Key.Name, retryInterval);
                            JobRunnerHelper.registerRetryJob(mainFactory, retryInterval, context.JobDetail);
                            TaskLogger.logRetryRegistered(context.JobDetail.Key.Name);
                        }
                        else
                        {
                            TaskLogger.logErrorTaskStart(context.JobDetail.Key.Name);
                            errorTask.executeTask(taskExecutionContext);
                            TaskLogger.logErrorTaskEnd(context.JobDetail.Key.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new TaskExecutionException(taskExecutionContext.StartAtTask, "executeTask", "Uncought exception when running", e);
                }

                TaskLogger.logJobRunnerEnd(jobName, context.NextFireTimeUtc);
                SystemLogger.logJobRunnerEnd(jobName, context.NextFireTimeUtc);
            }
            catch (Exception e)
            {
                string message = "JobRunner has thrown an exception";
                SystemLogger.logJobRunnerError(message, e);
                throw new JobExecutionException(message,e);
            }
        }

        public ITaskLogger TaskLogger
        {
            get
            {
                if (_taskLogger == null)
                {
                    throw new JobRunnerException("Unknown", "TaskLogger {get}", "Interface IJobRunner property TaskLogger called in an non-executing state");
                }
                else
                {
                    return _taskLogger;
                }
            }
        }

        public ISystemLogger SystemLogger
        {
            get
            {
                if (_systemLogger == null)
                {
                    throw new JobRunnerException("Unknown", "SystemLogger {get}", "Property SystemLogger called in an non-executing state");
                }
                else
                {
                    return _systemLogger;
                }
            }
        }

        public string CurrentTaskName
        {
            get
            {
                if (_jobExecutionContext == null)
                {
                    throw new JobRunnerException("Unknown", "CurrentTaskName {get}", "Interface IJobRunner property CurrentTaskName called in an non-executing state");
                }
                else
                {
                    return JobRunnerHelper.getCurrentTaskName(_jobExecutionContext);
                }
            }
            set
            {
                if (_jobExecutionContext == null)
                {
                    throw new JobRunnerException("Unknown", "CurrentTaskName {set}", "Interface IJobRunner property CurrentTaskName called in an non-executing state");
                }
                else
                {
                    TaskLogger.logTaskNameSet(_jobExecutionContext.JobDetail.Key.Name,value);
                    JobRunnerHelper.setCurrentTaskName(_jobExecutionContext, value);
                }
            }
        }

        public string JobName
        {
            get
            {
                if (_jobExecutionContext == null)
                {
                    throw new JobRunnerException("Unknown", "JobName {get}", "Interface IJobRunner property JobName called in an non-executing state");
                }
                else
                {
                    return JobRunnerHelper.getJobName(_jobExecutionContext);
                }
            }
        }

        public string LifetimeName
        {
            get
            {
                if (_jobExecutionContext == null)
                {
                    throw new JobRunnerException("Unknown", "LifetimeName {get}", "Interface IJobRunner property LifetimeName called in an non-executing state");
                }
                else
                {
                    return JobRunnerHelper.getLifetimeName(_jobExecutionContext);
                }
            }
        }

        public bool CanExecute
        {
            get { return true; }
        }
    }
}
