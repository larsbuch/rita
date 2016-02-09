using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Enumerations;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using RapidIntegrationTaskApplicationInterface.Variables;

namespace RapidIntegrationTaskApplicationExtension.VariableImplementations
{
    [Serializable]
    public class StringPHVariable : IStringVariable, IPlaceHolderVariable
    {
        private ITask _ownerTask;
        private IStringVariable _variable;

        public StringPHVariable(ITask ownerTask, string variableName, string parentVariableName)
        {
            _ownerTask = ownerTask;
            VariableName = variableName;
            IVariable variable = _ownerTask.ParentTask.getVariable(parentVariableName);
            if (variable is IStringVariable)
            {
                _variable = variable as IStringVariable;
            }
            else
            {
                throw new RapidIntegrationTaskApplicationException("Not IStringVariable");
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

        public void setValue(object value)
        {
            if (value is string)
            {
                _variable.setValue(value);
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a string");
            }
        }

        public bool Persisted
        {
            get { return _variable.Persisted; }
        }

        public string Value
        {
            get { return _variable.Value; }
            set { _variable.Value = value; }
        }

        public override string ToString()
        {
            return _variable.ToString();
        }

        public string ParentVariableName
        {
            get { return _variable.VariableName; }
        }

        public string getStringValue()
        {
            if (HasValue)
            {
                return Value;
            }
            else
            {
                throw new TaskExecutionException(OwnerTask.TaskName, "getStringValue()",
                                                 string.Format(
                                                     "String is null"));
            }
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
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a datetime value from {0} that is not a datetime", Type.ToString()));
        }

        public bool getBooleanValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a boolean value from {0} that is not a boolean", Type.ToString()));
        }

    }
}