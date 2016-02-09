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
    public class ListOfStringVariable : IListOfStringVariable
    {
        private List<string> _strings;
        private IVariableFactory _variableFactory;

        public ListOfStringVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName,
                                    bool persisted) : this(variableFactory, ownerTask, variableName, null, persisted)
        {
        }

        public ListOfStringVariable(IVariableFactory variableFactory, ITask ownerTask, string variableName,
                                    List<string> strings, bool persisted)
        {
            _variableFactory = variableFactory;
            VariableName = variableName;
            Type = VariableType.ListOfString;
            Persisted = persisted;
            OwnerTask = ownerTask;
            if (strings != null)
            {
                _strings = strings;
                if (Persisted)
                {
                    _variableFactory.settingPersistedVariable(this);
                }
            }
            else
            {
                _strings = new List<string>();
            }
        }

        public bool HasValue
        {
            get
            {
                if (_strings == null || _strings.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string VariableFullName
        {
            get { return OwnerTask.TaskFullName + Task.TaskChildNameSeparator + VariableName; }
        }

        public ITask OwnerTask { get; protected set; }
        public string VariableName { get; protected set; }
        public bool Persisted { get; protected set; }

        public VariableType Type { get; protected set; }

        public void addString(string newString)
        {
            _strings.Add(newString);
            if (Persisted)
            {
                _variableFactory.settingPersistedVariable(this);
            }
        }

        public void setValue(object value)
        {
            if (value is List<string>)
            {
                _strings = value as List<string>;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setValue", OwnerTask.TaskName, "Trying to se a value that is not a List<string>");
            }
        }
        
        public List<string> getStringList()
        {
            return _strings;
        }

        public override string ToString()
        {
            return string.Format("VariableName: {0} VariableType: {1} VariableValue {2}", VariableFullName, Type.ToString(),
                                 (HasValue
                                      ?
                                          string.Join(";", getStringList().ToArray())
                                      : "Empty List"));
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
                                                 "stringlist is null",
                                                 Type.ToString()));
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