using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Variables
{
    public interface IVariable
    {
        string VariableName { get; }
        Enumerations.VariableType Type { get; }
        bool Persisted { get; }
        bool HasValue { get;}
        string VariableFullName { get; }
        ITask OwnerTask { get; }
        void setValue(object value);
        string getStringValue();
        List<string> getStringListValue();
        int getIntegerValue();
        DateTime getDateTimeValue();
        bool getBooleanValue();
    }
}