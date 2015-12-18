using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Constants
{
    public static class BuildSchedule
    {
        // Xml Tags
        public static readonly string MaxRetry = "maxretry";
        public static readonly string RetryInterval = "retryinterval";
        public static readonly string JobName = "jobname";
        public static readonly string ErrorTasks = "errortasks";
        public static readonly string Tasks = "tasks";
        public static readonly string Task = "task";
        public static readonly string JobSchedule = "jobschedule";
        public static readonly string JobSchedules = "jobschedules";
        public static readonly string SubTasks = "subtasks";
        public static readonly string TaskName = "taskname";
        public static readonly string TaskClassName = "taskclassname";
        public static readonly string Variables = "variables";
        public static readonly string Variable = "variable";
        public static readonly string VariableType = "variabletype";
        public static readonly string VariableName = "variablename";
        public static readonly string VariableValue = "variablevalue";
        public static readonly string Value = "value";
        public static readonly string Triggers = "triggers";
        public static readonly string Trigger = "trigger";
        public static readonly string TriggerType = "triggertype";
        public static readonly string StartUTCDate = "startutcdate";
        public static readonly string StopUTCDate = "stoputcdate";
        public static readonly string Persisted = "persisted";

        // Specific Task
        public static readonly string GroupTaskName = "BasicTasks.GroupTask";
    }
}
