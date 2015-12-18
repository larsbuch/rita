using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerExtension.VariableImplementations
{
    [Serializable]
    public class BooleanPHVariable : IBooleanVariable, IPlaceHolderVariable
    {
        private ITask _ownerTask;
        private IBooleanVariable _variable;

        public BooleanPHVariable(ITask ownerTask, string variableName, string parentVariableName)
        {
            _ownerTask = ownerTask;
            IVariable variable = _ownerTask.ParentTask.getVariable(parentVariableName);
            VariableName = variableName;
            if (variable is IBooleanVariable)
            {
                _variable = variable as IBooleanVariable;
            }
            else
            {
                throw new RukisIntegrationTaskhandlerException("Not IBooleanVariable");
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

        public void setValue(object value)
        {
            if (value is bool?)
            {
                _variable.setValue(value);
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a bool?");
            }
        }

        public bool? Value
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
            throw new TaskExecutionException(OwnerTask.TaskName, "getStringValue()", string.Format("Trying to get a string value from {0} that is not a string", Type.ToString()));
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
            if (HasValue)
            {
                return Value.Value;
            }
            else
            {
                throw new TaskExecutionException(OwnerTask.TaskName, "getBooleanValue()", "Value is null");
            }
        }
    }
}