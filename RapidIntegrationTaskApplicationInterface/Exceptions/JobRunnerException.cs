using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Exceptions
{
    public class JobRunnerException : RukisIntegrationTaskhandlerException
    {
        public JobRunnerException(string jobName, string functionName, string message)
            : base(string.Format("Job: {0} Function: {1} Message: {2}", jobName, functionName, message))
        {
        }

        public JobRunnerException(string jobName, string functionName, string message, Exception innerException)
            : base(string.Format("JOb: {0} Function: {1} Message: {2}", jobName, functionName, message), innerException)
        {
        }
    }
}