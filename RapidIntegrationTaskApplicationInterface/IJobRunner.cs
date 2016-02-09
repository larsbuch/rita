using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IJobRunner:IJob
    {
        ITaskLogger TaskLogger { get; }
        string CurrentTaskName { get; set; }
        string JobName { get; }
        bool CanExecute { get; }
    }
}
