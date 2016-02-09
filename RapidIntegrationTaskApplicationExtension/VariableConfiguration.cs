using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Enumerations;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace RapidIntegrationTaskApplicationExtension
{
    public class VariableConfiguration:IVariableConfiguration
    {
        private IVariableFactory _variableFactory;
        private ITaskConfiguration _owningTaskConfiguration;

        public VariableConfiguration(IVariableFactory variableFactory, ITaskConfiguration owningTaskConfiguration, string name, VariableType variableType, bool persisted, string comment)
        {
            VariableName = name;
            VariableType = variableType;
            Persisted = persisted;
            Comment = comment;
            _variableFactory = variableFactory;
            _owningTaskConfiguration = owningTaskConfiguration;
        }

        public string Comment { get; protected set; }
        public string VariableName { get; protected set; }
        public VariableType VariableType { get; set;}
        public bool Persisted { get; set; }
        public object Value { get; set;}
        public string FullName 
        { 
            get 
            { 
                if(_owningTaskConfiguration == null)
                {
                    return VariableName;
                }
                else
                {
                    return _owningTaskConfiguration.FullName + Task.TaskChildNameSeparator + VariableName;
                }
            }
        }

        public IVariableConfiguration getCopy(ITaskConfiguration owningTaskConfiguration,bool removePersistedValues)
        {
            return _variableFactory.copyVariableConfiguration(owningTaskConfiguration,this, removePersistedValues);
        }

        public void setOwningTaskConfiguration(ITaskConfiguration owningTaskConfiguration)
        {
            _owningTaskConfiguration = owningTaskConfiguration;
        }

        public bool isUnconfigured()
        {
            if (Value == null || Value.Equals(Task.UnconfiguredVariable))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
