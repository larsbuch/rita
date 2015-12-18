using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface.Exceptions;

namespace RukisIntegrationTaskhandlerInterface
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
