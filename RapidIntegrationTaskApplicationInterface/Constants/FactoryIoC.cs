using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Constants
{
    public static class FactoryIoC
    {
        public static readonly string QuartzSchedulerFactory = "QuartzSchedulerFactory";
        public static readonly string SystemLogger = "SystemLogger";
        public static readonly string TaskLogger = "TaskLogger";
        public static readonly string JobScheduleFactory = "JobScheduleFactory";
        public static readonly string JobRunnerFactory = "JobRunnerFactory";
        public static readonly string TaskFactory = "TaskFactory";
        public static readonly string VariableFactory = "VariableFactory";
        public static readonly string JobTriggerFactory = "JobTriggerFactory";
        public static readonly string TaskTemplateWriter = "TaskTemplateWriter";
        public static readonly string VariablePersister = "VariablePersister";
    }
}
