﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobRunnerDummy : IJobRunner
    {
        public void Execute(IJobExecutionContext context)
        {
            throw new JobRunnerException(JobName, "Execute", "Executing is not allowed");
        }

        public ITaskLogger TaskLogger
        {
            get { throw new JobRunnerException(JobName, "TaskLogger", "Getting Tasklogger is not allowed"); }
        }

        public string CurrentTaskName
        {
            get { throw new JobRunnerException(JobName, "CurrentTaskName", "Getting CurrentTaskName is not allowed"); }
            set { throw new JobRunnerException(JobName, "CurrentTaskName", "Setting CurrentTaskName is not allowed"); }
        }

        public string JobName
        {
            get { return "JobRunnerDummy"; }
        }

        public bool CanExecute
        {
            get { return false; }
        }
    }
}