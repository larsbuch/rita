using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IJobRunnerFactory
    {
        IMainFactory MainFactory { get; set; }
        Type getJobRunnerType();
        IJobDetail getNewJobRunner(IJobSchedule jobSchedule);
        IJobDetail getNewRetryJobRunner(IJobDetail originalJobDetail);
        IJobRunner getDummyJobRunner();
    }
}
