using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Spring.Context;
using Spring.Context.Support;

namespace RukisIntegrationTaskhandlerExtension
{
    public class MainFactory:IMainFactory
    {
        private static IMainFactory _mainFactory;

        private MainFactory()
        {
                configureFactory();
        }

        public static IMainFactory Current
        {
            get
            {
                if (_mainFactory == null)
                {
                    _mainFactory = new MainFactory();
                }
                return _mainFactory;
            }
        }

        private void configureFactory()
        {
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            ISchedulerFactory scheduleFactory =
                (ISchedulerFactory)
                applicationContext.GetObject(FactoryIoC.QuartzSchedulerFactory, typeof(ISchedulerFactory));
            _scheduler = scheduleFactory.GetScheduler();
            _systemLogger =
                (ISystemLogger)applicationContext.GetObject(FactoryIoC.SystemLogger, typeof(ISystemLogger));
            _taskLogger =
                (ITaskLogger)applicationContext.GetObject(FactoryIoC.TaskLogger, typeof(ITaskLogger));
            _jobScheduleFactory =
                (IJobScheduleFactory)
                applicationContext.GetObject(FactoryIoC.JobScheduleFactory, typeof(IJobScheduleFactory));
            _jobRunnerFactory =
                (IJobRunnerFactory)
                applicationContext.GetObject(FactoryIoC.JobRunnerFactory, typeof(IJobRunnerFactory));
            _taskFactory = (ITaskFactory)
                           applicationContext.GetObject(FactoryIoC.TaskFactory, typeof(ITaskFactory));
            _jobTriggerFactory = (IJobTriggerFactory)applicationContext.GetObject(FactoryIoC.JobTriggerFactory, typeof(IJobTriggerFactory));
            _variableFactory = (IVariableFactory)applicationContext.GetObject(FactoryIoC.VariableFactory, typeof(IVariableFactory));
            _taskTemplateWriter =
                (ITaskTemplateWriter)
                applicationContext.GetObject(FactoryIoC.TaskTemplateWriter, typeof(ITaskTemplateWriter));
            _variablePersister =
                (IVariablePersister)
                applicationContext.GetObject(FactoryIoC.VariablePersister, typeof(IVariablePersister));

            // Set main factory
            _jobScheduleFactory.MainFactory = this;
            _jobRunnerFactory.MainFactory = this;
            _taskFactory.MainFactory = this;
            _jobTriggerFactory.MainFactory = this;
            _variableFactory.MainFactory = this;
        }

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

        public void unloadFactory()
        {
            Scheduler.Shutdown();
            SystemLogger.logServiceStop();
            _mainFactory = null;
        }

        private void registerSchedules()
        {
            try
            {
                List<IJobSchedule> jobSchedules = JobScheduleFactory.getJobSchedules();
                foreach (IJobSchedule jobSchedule in jobSchedules)
                {
                    JobDetail jobDetail = JobRunnerFactory.getNewJobRunner(jobSchedule);

                    List<Trigger> jobTriggers = JobTriggerFactory.getJobTriggers(jobSchedule.getTriggerConfigurations());

                    foreach (Trigger jobTrigger in jobTriggers)
                    {
                        Scheduler.ScheduleJob(jobDetail, jobTrigger);
                        SystemLogger.logScheduleStarted(jobDetail.Name, jobTrigger.Name, jobTrigger.GetNextFireTimeUtc());
                    }
                }
            }
            catch (Exception e)
            {
                SystemLogger.logRegisteringSchedulesError(e);
                throw new RukisIntegrationTaskhandlerException("Error occured while registering schedules", e);
            }
        }

    }
}
