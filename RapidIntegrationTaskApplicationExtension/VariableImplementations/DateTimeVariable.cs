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
    public class DateTimeVariable : IDateTimeVariable
    {
        private DateTime? _value;
        private IVariableFactory _variableFactory;

        public DateTimeVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName,
                                DateTime? dateTime, bool persisted)
        {
            _variableFactory = variableFactory;
            VariableName = variableName;
            Value = dateTime;
            Type = VariableType.DateTime;
            Persisted = persisted;
            OwnerTask = ownerTask;
        }

        public DateTimeVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName,
                                string dateTimeString,
                                bool persisted)
            : this(variableFactory, ownerTask, variableName, getDateTime(dateTimeString), persisted)
        {
        }

        private static DateTime? getDateTime(string dateTimeString)
        {
            if (dateTimeString == null || dateTimeString.Equals(string.Empty))
            {
                return null;
            }
            else
            {
                return FactoryHelper.getDateTime(dateTimeString, true);
            }
        }

        public bool HasValue
        {
            get { return Value.HasValue; }
        }

        public string VariableFullName
        {
            get { return OwnerTask.TaskFullName + Task.TaskChildNameSeparator + VariableName; }
        }

        public ITask OwnerTask { get; protected set; }
        public string VariableName { get; protected set; }
        public bool Persisted { get; protected set; }
        public VariableType Type { get; protected set; }

        public DateTime? Value
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
        public void setValue(object value)
        {
            if (value is DateTime?)
            {
                Value = value as DateTime?;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a DateTime?");
            }
        }

        public void setDateTimeFromString(string dateTimeString)
        {
            if (dateTimeString == null || dateTimeString.Equals(string.Empty))
            {
                Value = null;
            }
            else
            {
                Value = FactoryHelper.getDateTime(dateTimeString, true);
            }
        }

        public string getDateTimeAsString()
        {
            if (HasValue)
            {
                return FactoryHelper.getDateTimeString(Value.Value);
            }
            else
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            return string.Format("VariableName: {0} VariableType: {1} VariableValue {2}", VariableFullName, Type.ToString(),
                                 getDateTimeAsString());
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
            if (HasValue)
            {
                return Value.Value;
            }
            else
            {
                throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", "Value is null");
            }
        }

        public bool getBooleanValue()
        {
            throw new TaskExecutionException(OwnerTask.TaskName, "getDateTimeValue()", string.Format("Trying to get a boolean value from {0} that is not a boolean", Type.ToString()));
        }

    }
}