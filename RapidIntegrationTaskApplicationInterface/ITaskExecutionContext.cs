using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ITaskExecutionContext
    {
        bool StartTaskFound { get; set; }
        string StartAtTask{ get;}
        ITaskLogger Log { get; }
        void setCurrentlyExecutingTask(string taskName);
        string JobName { get; }
    }
}
