using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerExtension.VariableImplementations
{
    [Serializable]
    public class DateTimePHVariable : IDateTimeVariable, IPlaceHolderVariable
    {
        private ITask _ownerTask;
        private IDateTimeVariable _variable;

        public DateTimePHVariable(ITask ownerTask, string variableName, string parentVariableName)
        {
            _ownerTask = ownerTask;
            IVariable variable = _ownerTask.ParentTask.getVariable(parentVariableName);
            VariableName = variableName;
            if (variable is IDateTimeVariable)
            {
                _variable = variable as IDateTimeVariable;
            }
            else
            {
                throw new RukisIntegrationTaskhandlerException("Not IDateTimeVariable");
            }
        }

        public string VariableName { get; protected set; }

        public string VariableFullName
        {
            get { return _variable.VariableFullName; }
        }

        public ITask OwnerTask
        {
            get { return _variable.OwnerTask; }
        }

        public VariableType Type
        {
            get { return _variable.Type; }
        }

        public bool HasValue
        {
            get { return _variable.HasValue; }
        }

        public bool Persisted
        {
            get { return _variable.Persisted; }
        }

        public DateTime? Value
        {
            get { return _variable.Value; }
            set { _variable.Value = value; }
        }

        public void setValue(object value)
        {
            if (value is DateTime?)
            {
                _variable.Value = value as DateTime?;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a DateTime?");
            }
        }

        public string getStringValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName,"getStringValue()",string.Format("Trying to get a string value from {0} that is not a string",Type.ToString()));
        }

        public List<string> getStringListValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getStringListValue()", string.Format("Trying to get a stringlist value from {0} that is not a stringlist", Type.ToString()));
        }

        public int getIntegerValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getIntegerValue()", string.Format("Trying to get a integer value from {0} that is not a integer", Type.ToString()));
        }

        public DateTime getDateTimeValue()
        {
            if (HasValue)
            {
                return Value.Value;
            }
            else
            {
                throw new TaskExecutionException(OwnerTask.TaskName,"getDateTimeValue()", "Value is null");
            }
        }

        public void setDateTimeFromString(string dateTimeString)
        {
            _variable.setDateTimeFromString(dateTimeString);
        }

        public string getDateTimeAsString()
        {
            return _variable.getDateTimeAsString();
        }

        public override string ToString()
        {
            return _variable.ToString();
        }

        public string ParentVariableName
        {
            get { return _variable.VariableName; }
        }

        public bool getBooleanValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a boolean value from {0} that is not a boolean", Type.ToString()));
        }

    }
}