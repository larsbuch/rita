using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Autofac;

namespace RapidIntegrationTaskApplicationExtension
{
    public class MainFactory:IMainFactory
    {

        #region Static

        private static Dictionary<string, IMainFactory> _mainFactories;

        private static Dictionary<string, IMainFactory> MainFactoryList
        {
            get
            {
                if (_mainFactories == null)
                {
                    _mainFactories = new Dictionary<string, IMainFactory>();
                }
                return _mainFactories;
            }
        }

        public static IMainFactory GetMainFactory(string lifetimeName)
        {
            IMainFactory mainFactory;
            if (MainFactoryList.TryGetValue(lifetimeName, out mainFactory))
            {
                return mainFactory;
            }
            else
            {
                mainFactory = new MainFactory(lifetimeName);
                MainFactoryList.Add(lifetimeName, mainFactory);
                return mainFactory;
            }
        }

        #endregion

        private MainFactory(string lifetimeName)
        {
            _lifetimeName = lifetimeName;
            buildIoC();
            configureFactory();
        }

        private void buildIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<StdSchedulerFactory>().As<ISchedulerFactory>();
            builder.RegisterType<SystemLogger>().As<ISystemLogger>();
            builder.RegisterType<TaskLogger>().As<ITaskLogger>();
            builder.RegisterType<JobScheduleFactory>().As<IJobScheduleFactory>();
            builder.RegisterType<JobRunnerFactory>().As<IJobRunnerFactory>();
            builder.RegisterType<TaskFactory>().As<ITaskFactory>();
            builder.RegisterType<JobTriggerFactory>().As<IJobTriggerFactory>();
            builder.RegisterType<VariableFactory>().As<IVariableFactory>();
            builder.RegisterType<TaskTemplateWriter>().As<ITaskTemplateWriter>();
            builder.RegisterType<VariablePersister>().As<IVariablePersister>();
            _iocContainer = builder.Build();
        }

        private void configureFactory()
        {
            ISchedulerFactory scheduleFactory = _iocContainer.Resolve<ISchedulerFactory>();
            _scheduler = scheduleFactory.GetScheduler();
            _systemLogger = _iocContainer.Resolve<ISystemLogger>();
            _taskLogger = _iocContainer.Resolve<ITaskLogger>();
            _jobScheduleFactory = _iocContainer.Resolve<IJobScheduleFactory>();
            _jobRunnerFactory = _iocContainer.Resolve<IJobRunnerFactory>();
            _taskFactory = _iocContainer.Resolve<ITaskFactory>();
            _jobTriggerFactory = _iocContainer.Resolve<IJobTriggerFactory>();
            _variableFactory = _iocContainer.Resolve<IVariableFactory>();
            _taskTemplateWriter = _iocContainer.Resolve<ITaskTemplateWriter>();
            _variablePersister = _iocContainer.Resolve<IVariablePersister>();

            // Set main factory
            _jobScheduleFactory.MainFactory = this;
            _jobRunnerFactory.MainFactory = this;
            _taskFactory.MainFactory = this;
            _jobTriggerFactory.MainFactory = this;
            _variableFactory.MainFactory = this;
        }

        private string _lifetimeName;
        private IJobTriggerFactory _jobTriggerFactory;
        private  IJobScheduleFactory _jobScheduleFactory;
        private  IJobRunnerFactory _jobRunnerFactory;
        private  ITaskFactory _taskFactory;
        private  ISystemLogger _systemLogger;
        private  ITaskLogger _taskLogger;
        private  IScheduler _scheduler;
        private IVariableFactory _variableFactory;
        private ITaskTemplateWriter _taskTemplateWriter;
        private IVariablePersister _variablePersister;
        private IContainer _iocContainer;

        #region Properties

        public IContainer IoCContainer
        {
            get
            {
                return _iocContainer;
            }
        }

        public string LifetimeName
        {
            get
            {
                return _lifetimeName;
            }
        }

        public IVariablePersister VariablePersister
        {
            get
            {
                return _variablePersister;
            }
        }

        public IScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
        }

        public ISystemLogger SystemLogger
        {
            get
            {
                return _systemLogger;
            }
        }

        public ITaskLogger TaskLogger
        {
            get
            {
                return _taskLogger;
            }
        }

        public ITaskFactory TaskFactory
        {
            get
            {
                return _taskFactory;
            }
        }

        public IJobRunnerFactory JobRunnerFactory
        {
            get
            {
                return _jobRunnerFactory;
            }
        }

        public IJobScheduleFactory JobScheduleFactory
        {
            get
            {
                return _jobScheduleFactory;
            }
        }

        public IJobTriggerFactory JobTriggerFactory
        {
            get
            {
                return _jobTriggerFactory;
            }
        }

        public IVariableFactory VariableFactory
        {
            get
            {
                return _variableFactory;
            }
        }

        #endregion

        public void loadAndConfigureFactory()
        {
            SystemLogger.logServiceStart();
            try
            {
                // Ready Task Factory
                TaskFactory.loadFactory();
                SystemLogger.logTaskFactoryInitialized();

                // Export TaskTemplates
                _taskTemplateWriter.writeTaskTemplates(TaskFactory.getDefaultTaskConfigurationList());
                SystemLogger.logTaskTemplateWritten();

                // Start Scheduler
                Scheduler.Start();
                SystemLogger.logSchedulerStart();

                //Configure jobs
                registerSchedules();
                SystemLogger.logJobsConfigured();
            }
            catch (Exception e)
            {
                SystemLogger.logServiceStartupError(e);
                Scheduler.Shutdown();
                throw;
            }

        }

        public void Dispose()
        {
            Scheduler.Shutdown();
            SystemLogger.logServiceStop();
            _mainFactories.Remove(LifetimeName);
        }

        private void registerSchedules()
        {
            try
            {
                List<IJobSchedule> jobSchedules = JobScheduleFactory.getJobSchedules();
                foreach (IJobSchedule jobSchedule in jobSchedules)
                {
                    IJobDetail jobDetail = JobRunnerFactory.getNewJobRunner(jobSchedule);

                    List<ITrigger> jobTriggers = JobTriggerFactory.getJobTriggers(jobSchedule.getTriggerConfigurations());

                    foreach (ITrigger jobTrigger in jobTriggers)
                    {
                        Scheduler.ScheduleJob(jobDetail, jobTrigger);
                        SystemLogger.logScheduleStarted(jobDetail.Key.Name, jobTrigger.Key.Name, jobTrigger.GetNextFireTimeUtc());
                    }
                }
            }
            catch (Exception e)
            {
                SystemLogger.logRegisteringSchedulesError(e);
                throw new RapidIntegrationTaskApplicationException("Error occured while registering schedules", e);
            }
        }

    }
}
