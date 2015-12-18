using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface IMainFactory
    {
        IScheduler Scheduler { get; }
        ISystemLogger SystemLogger { get; }
        ITaskLogger TaskLogger { get; }
        ITaskFactory TaskFactory { get; }
        IJobRunnerFactory JobRunnerFactory { get; }
        IJobScheduleFactory JobScheduleFactory { get; }
        IJobTriggerFactory JobTriggerFactory { get; }
        IVariableFactory VariableFactory { get; }
        IVariablePersister VariablePersister { get; }
        void loadAndConfigureFactory();
        void unloadFactory();
    }
}
