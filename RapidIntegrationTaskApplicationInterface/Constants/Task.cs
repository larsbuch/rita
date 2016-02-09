using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Constants
{
    public static class Task
    {
        public static readonly string RetryPrefix = "Retry_";
        public static readonly string TaskChildNameSeparator = ":";
        public static readonly string StartFromFirstTask = "";
        public static readonly string ParentExecutionTask = "BasicTasks.ParentExecutionTask";
        public static readonly string UnconfiguredVariable = "UnconfiguredVariable";
        public static readonly string UnconfiguredTaskName = "UnconfiguredTaskName";
        public static readonly string ParentTaskVariable = "ParentTaskVariable";
    }
}
