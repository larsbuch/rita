using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace RapidIntegrationTaskApplicationExtension
{
    public class TaskExecutionContext:ITaskExecutionContext
    {
        private readonly IJobRunner _jobRunner;
        private readonly string _startAtTaskName;

        public TaskExecutionContext(IJobRunner jobRunner, string startAtTaskName)
        {
            if(startAtTaskName.Equals(Task.StartFromFirstTask))
            {
                StartTaskFound = true;
            }
            else
            {
                StartTaskFound = false;
            }
            if (jobRunner == null)
            {
                throw new JobRunnerException("Unknown", "Constructor TaskExecutionContext", "jobRunner is null in TaskExecutionContext");
            }
            else
            {
                _jobRunner = jobRunner;
            }
            if (startAtTaskName == null)
            {
                throw new JobRunnerException(jobRunner.JobName, "Construction TaskExecutionContext","startAtTaskName is null in TaskExecutionContext");
            }
            else
            {
                _startAtTaskName = startAtTaskName;
            }
        }

        public bool StartTaskFound { get; set;}

        public string StartAtTask 
        {
            get { return _startAtTaskName; }
        }

        public ITaskLogger Log
        {
            get { return _jobRunner.TaskLogger; }
        }

        public void setCurrentlyExecutingTask(string taskName)
        {
            _jobRunner.CurrentTaskName = taskName;
        }

        public string JobName
        {
            get { return _jobRunner.JobName; }
        }
    }
}
