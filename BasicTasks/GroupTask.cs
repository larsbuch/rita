using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicTasks;
using RapidIntegrationTaskApplicationInterface;

namespace BasicTasks
{
    public class GroupTask : AbstractTask
    {
        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            executeChildTasks(ChildTasks,taskExecutionContext);
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