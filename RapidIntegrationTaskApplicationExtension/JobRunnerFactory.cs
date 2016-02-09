using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobRunnerFactory:IJobRunnerFactory
    {
        public IMainFactory MainFactory { get; set; }

        public Type getJobRunnerType()
        {
            return typeof (JobRunner);
        }

        public IJobDetail getNewJobRunner(IJobSchedule jobSchedule)
        {
            IJobDetail jobDetail = JobBuilder.Create().WithIdentity(jobSchedule.getJobName()).OfType(getJobRunnerType()).Build();
            JobRunnerHelper.setContext(MainFactory.LifetimeName, jobDetail, jobSchedule.getTaskConfiguration(), jobSchedule.getErrorTaskConfiguration(),
                                     jobSchedule.getRetryInterval(), jobSchedule.getMaxRetry());
            return jobDetail;
        }

        public IJobDetail getNewRetryJobRunner(IJobDetail originalJobDetail)
        {
            string newJobName = originalJobDetail.Key.Name;
            if (!newJobName.StartsWith(Task.RetryPrefix))
            {
                newJobName = Task.RetryPrefix + newJobName + "_1";
            }
            else
            {
                int currentRetry = JobRunnerHelper.getCurrentRetry(originalJobDetail);
                int lastRetry = currentRetry - 1;
                newJobName = newJobName.Replace("_" + lastRetry, "_" + currentRetry);
            }

            // Make random group to avoid clash
            string randomGroupName = System.Guid.NewGuid().ToString();

            IJobDetail jobDetail = JobBuilder.Create().WithIdentity(newJobName, randomGroupName).OfType(getJobRunnerType()).Build();
            JobRunnerHelper.makeRetryContext(GetType().Name, jobDetail, originalJobDetail);
            return jobDetail;
        }

        public IJobRunner getDummyJobRunner()
        {
            return new JobRunnerDummy();
        }
    }
}
