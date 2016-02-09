using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Exceptions
{
    public class TaskExecutionException : RapidIntegrationTaskApplicationException
    {
        public TaskExecutionException(string taskName, string functionName, string message)
            : base(string.Format("Task: {0} Function: {1} Message: {2}", taskName, functionName, message))
        {
        }

        public TaskExecutionException(string taskName, string functionName, string message, Exception innerException)
            : base(string.Format("Task: {0} Function: {1} Message: {2}", taskName, functionName, message), innerException)
        {
        }
    }
}
