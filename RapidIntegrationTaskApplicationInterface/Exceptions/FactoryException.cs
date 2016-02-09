using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Exceptions
{
    public class FactoryException : RapidIntegrationTaskApplicationException
    {
        public FactoryException(string factoryName, string functionName, string message)
            : base(string.Format("FactoryName: {0} Function: {1} Message: {2}", factoryName, functionName, message))
        {
        }

        public FactoryException(string factoryName, string functionName, string message, Exception innerException)
            : base(
                string.Format("FactoryName: {0} Function: {1} Message: {2}", factoryName, functionName, message),
                innerException)
        {
        }
    }
}