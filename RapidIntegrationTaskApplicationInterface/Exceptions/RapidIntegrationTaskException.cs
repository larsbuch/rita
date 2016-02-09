using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Exceptions
{
    public class RapidIntegrationTaskApplicationException : ApplicationException
    {
        public RapidIntegrationTaskApplicationException()
            : base()
        {
        }

        public RapidIntegrationTaskApplicationException(string message)
            : base(message)
        {
        }

        public RapidIntegrationTaskApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}