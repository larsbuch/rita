using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface IJobScheduleFactory
    {
        IMainFactory MainFactory { get; set; }
        List<IJobSchedule> getJobSchedules();
        IJobSchedule createNewJobSchedule();
    }
}
