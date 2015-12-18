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
    public class IntegerVariable : IIntegerVariable
    {
        private IVariableFactory _variableFactory;
        private int? _value;

        public IntegerVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName, int? value,
                               bool persisted)
        {
            _variableFactory = variableFactory;
            VariableName = variableName;
            Value = value;
            Type = VariableType.Integer;
            Persisted = persisted;
            OwnerTask = ownerTask;
        }

        public bool HasValue
        {
            get { return Value.HasValue; }
        }

        public int? Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (Persisted)
                {
                    _variableFactory.settingPersistedVariable(this);
                }
            }
        }

        public string VariableFullName
        {
            get { return OwnerTask.TaskFullName + Task.TaskChildNameSeparator + VariableName; }
        }
        public void setValue(object value)
        {
            if (value is int?)
            {
                Value = value as int?;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a int?");
            }
        }

        public ITask OwnerTask { get; protected set; }
        public string VariableName { get; protected set; }
        public VariableType Type { get; protected set; }
        public bool Persisted { get; protected set; }

        public override string ToString()
        {
            return string.Format("VariableName: {0} VariableType: {1} VariableValue {2}", VariableFullName, Type.ToString(),
                                 (Value.HasValue
                                      ?
                                          Value.Value.ToString()
                                      : "Empty"));
        }

        public string getStringValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getStringValue()", string.Format("Trying to get a string value from {0} that is not a string", Type.ToString()));
        }

        public List<string> getStringListValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getStringListValue()", string.Format("Trying to get a stringlist value from {0} that is not a stringlist", Type.ToString()));
        }

        public DateTime getDateTimeValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a datetime value from {0} that is not a datetime", Type.ToString()));
        }

        public int getIntegerValue()
        {
            if (HasValue)
            {
                return Value.Value;
            }
            else
            {
                throw new TaskExecutionException(OwnerTask.TaskName, "getIntegerValue()", string.Format("Value is null"));
            }
        }

        public bool getBooleanValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a boolean value from {0} that is not a boolean", Type.ToString()));
        }

    }
}