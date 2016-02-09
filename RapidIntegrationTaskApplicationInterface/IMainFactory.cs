using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Autofac;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IMainFactory: IDisposable
    {
        string LifetimeName { get; }
        IContainer IoCContainer { get; }
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
    }
}
