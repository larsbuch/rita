using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using Quartz;

namespace RukisIntegrationTaskhandlerExtension
{
    public class JobRunnerFactory:IJobRunnerFactory
    {
        public IMainFactory MainFactory { get; set; }

        public Type getJobRunnerType()
        {
            return typeof (JobRunner);
        }

        public JobDetail getNewJobRunner(IJobSchedule jobSchedule)
        {
            JobDetail jobDetail = new JobDetail(jobSchedule.getJobName(), null, getJobRunnerType());

            JobRunnerHelper.setContext(jobDetail, jobSchedule.getTaskConfiguration(), jobSchedule.getErrorTaskConfiguration(),
                                     jobSchedule.getRetryInterval(), jobSchedule.getMaxRetry());
            return jobDetail;
        }

        public JobDetail getNewRetryJobRunner(JobDetail originalJobDetail)
        {
            string newJobName = originalJobDetail.Name;
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

            JobDetail jobDetail = new JobDetail(newJobName, null, getJobRunnerType());
            jobDetail.Group = randomGroupName;
            JobRunnerHelper.makeRetryContext(GetType().Name, jobDetail, originalJobDetail);
            return jobDetail;
        }

        public IJobRunner getDummyJobRunner()
        {
            return new JobRunnerDummy();
        }
    }
}
