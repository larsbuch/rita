using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Exceptions
{
    public class RukisIntegrationTaskhandlerException : ApplicationException
    {
        public RukisIntegrationTaskhandlerException()
            : base()
        {
        }

        public RukisIntegrationTaskhandlerException(string message)
            : base(message)
        {
        }

        public RukisIntegrationTaskhandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}