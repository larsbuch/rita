using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Variables
{
    public interface IDateTimeVariable:IVariable
    {
        DateTime? Value { get; set; }
        void setDateTimeFromString(string dateTimeString);
        string getDateTimeAsString();
    }
}
