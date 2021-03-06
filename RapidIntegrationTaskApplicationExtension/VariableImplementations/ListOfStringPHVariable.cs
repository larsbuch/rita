﻿using System;
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
    public class ListOfStringPHVariable : IListOfStringVariable, IPlaceHolderVariable
    {
        private ITask _ownerTask;
        private IListOfStringVariable _variable;

        public ListOfStringPHVariable(ITask ownerTask, string variableName, string parentVariableName)
        {
            _ownerTask = ownerTask;
            IVariable variable = _ownerTask.ParentTask.getVariable(parentVariableName);
            VariableName = variableName;
            if (variable is IListOfStringVariable)
            {
                _variable = variable as IListOfStringVariable;
            }
            else
            {
                throw new RapidIntegrationTaskApplicationException("Not IListOfStringVariable");
            }
        }

        public string VariableFullName
        {
            get { return _variable.VariableFullName; }
        }

        public ITask OwnerTask
        {
            get { return _variable.OwnerTask; }
        }

        public string VariableName { get; protected set;}

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

        public void addString(string newString)
        {
            _variable.addString(newString);
        }

        public void setValue(object value)
        {
            if (value is List<string>)
            {
                _variable.setValue(value);
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a List<string>");
            }
        }

        public List<string> getStringList()
        {
            return _variable.getStringList();
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
            if (HasValue)
            {
                return getStringList();
            }
            else
            {

                throw new TaskExecutionException(OwnerTask.TaskName, "getStringListValue()",
                                                 string.Format(
                                                     "Value is null"));
            }
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