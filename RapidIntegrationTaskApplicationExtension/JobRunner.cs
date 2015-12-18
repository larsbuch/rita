using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using Common.Logging;
using Quartz;

namespace RukisIntegrationTaskhandlerExtension
{
    public class JobRunner:IJobRunner
    {
        private ITaskLogger _taskLogger = null;
        private ISystemLogger _systemLogger = null;
        private JobExecutionContext _jobExecutionContext = null;

        public void Execute(JobExecutionContext context)
        {
            try
            {
                // Set initial values
                _jobExecutionContext = context;
                _taskLogger = MainFactory.Current.TaskLogger;
                _systemLogger = MainFactory.Current.SystemLogger;

                string jobName = JobRunnerHelper.getJobName(context);

                TaskLogger.logJobRunnerConfiguring(jobName);

                ITaskConfiguration taskConfiguration = JobRunnerHelper.getTaskConfiguration(context);
                ITaskConfiguration errorTaskConfiguration = JobRunnerHelper.getErrorTaskConfiguration(context);
                int retryInterval = JobRunnerHelper.getRetryInterval(context);
                string startAtTask = JobRunnerHelper.getCurrentTaskName(context);

                ITaskExecutionContext taskExecutionContext = new TaskExecutionContext(this, startAtTask);

                ITask task = MainFactory.Current.TaskFactory.buildTask(this,taskConfiguration);
                ITask errorTask = MainFactory.Current.TaskFactory.buildTask(this,errorTaskConfiguration);

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
                            TaskLogger.logRetryRegistering(context.JobDetail.Name, retryInterval);
                            JobRunnerHelper.registerRetryJob(MainFactory.Current, retryInterval, context.JobDetail);
                            TaskLogger.logRetryRegistered(context.JobDetail.Name);
                        }
                        else
                        {
                            TaskLogger.logErrorTaskStart(context.JobDetail.Name);
                            errorTask.executeTask(taskExecutionContext);
                            TaskLogger.logErrorTaskEnd(context.JobDetail.Name);
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
                    TaskLogger.logTaskNameSet(_jobExecutionContext.JobDetail.Name,value);
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
                    throw new JobRunnerException("Unknown","JobName {get}","Interface IJobRunner property JobName called in an non-executing state");
                }
                else
                {
                    return JobRunnerHelper.getJobName(_jobExecutionContext);
                }
            }
        }

        public bool CanExecute
        {
            get { return true; }
        }
    }
}
