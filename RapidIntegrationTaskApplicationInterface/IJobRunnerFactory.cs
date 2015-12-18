using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface IJobRunnerFactory
    {
        IMainFactory MainFactory { get; set; }
        Type getJobRunnerType();
        JobDetail getNewJobRunner(IJobSchedule jobSchedule);
        JobDetail getNewRetryJobRunner(JobDetail originalJobDetail);
        IJobRunner getDummyJobRunner();
    }
}
