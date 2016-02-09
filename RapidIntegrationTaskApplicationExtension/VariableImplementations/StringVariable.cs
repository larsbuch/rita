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
    public class StringVariable : IStringVariable
    {
        private IVariableFactory _variableFactory;
        private string _value;

        public StringVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName,
                              string variableValue, bool persisted)
        {
            _variableFactory = variableFactory;
            VariableName = variableName;
            Value = variableValue;
            Type = VariableType.String;
            Persisted = persisted;
            OwnerTask = ownerTask;
        }

        public string Value
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

        public ITask OwnerTask { get; protected set; }
        public void setValue(object value)
        {
            if (value is string)
            {
                Value = value as string;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name,"setValue",OwnerTask.TaskName,"Trying to se a value that is not a string");
            }
        }

        public string VariableName { get; protected set; }
        public VariableType Type { get; protected set; }
        public bool Persisted { get; protected set; }

        public bool HasValue
        {
            get
            {
                if (Value == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("VariableName: {0} VariableType: {1} VariableValue {2}", VariableFullName, Type.ToString(),
                                 (HasValue
                                      ?
                                          Value
                                      : "Null String"));
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