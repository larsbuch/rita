using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Exceptions
{
    public class TaskConfigurationException : RapidIntegrationTaskApplicationException
    {
        public TaskConfigurationException(string className, string functionName,string taskName, string message)
            : base(string.Format("Class: {0} Function: {1} Task: {2} Message: {3}",className,functionName,taskName,message))
        {
        }

        public TaskConfigurationException(string className, string functionName, string taskName, string message, Exception innerException)
            : base(string.Format("Class: {0} Function: {1} Task: {2} Message: {3}",className,functionName,taskName,message),innerException)
        {
        }
    }
}
