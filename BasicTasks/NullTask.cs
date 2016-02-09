using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicTasks;
using RapidIntegrationTaskApplicationInterface;

namespace BasicTasks
{
    public class NullTask : AbstractTask
    {
        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
            taskExecutionContext.Log.logDebugText(string.Format("Task {0} ({1}) in job {2} is executing.",TaskName,TaskFullName,taskExecutionContext.JobName));
        }

        protected override void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        public override void resetTask()
        {
            // Do Nothing
        }
    }
}