using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicTasks;
using ChronographedTaskInterface;
using ChronographedTaskInterface.Exceptions;

namespace BasicTasks
{
    public class OccationallyFailingTask:AbstractTask
    {
        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing except fail occationally
            Random random = new Random();
            int value = random.Next(10);

            if (value <= 1)
            {
                throw new TaskExecutionException(TaskName, "executeTaskInternally", "Testing executeTaskInternally failing");
            }
        }

        protected override void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        public override void resetTask()
        {
            // Do Nothing except fail occationally
            Random random = new Random();
            int value = random.Next(10);

            if (value <= 1)
            {
                throw new TaskExecutionException(TaskName,"resetTask","Testing resetTask failing");
            }
        }
    }
}