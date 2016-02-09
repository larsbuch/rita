using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using RapidIntegrationTaskApplicationExtension.VariableImplementations;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Enumerations;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using RapidIntegrationTaskApplicationInterface.Variables;
using Common.Logging;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public class VariableFactory : IVariableFactory
    {
        public IMainFactory MainFactory { get; set; }

        public VariableFactory()
        {
        }

        protected IVariablePersister getVariablePersister()
        {
            return MainFactory.VariablePersister;
        }

        protected void registerPersistedVariable(IVariable variable)
        {
            getVariablePersister().registerPersistedVariable(variable);
        }

        public void settingPersistedVariable(IVariable variable)
        {
            getVariablePersister().settingPersistedVariable(variable);
        }


        #region Task Support

        public IVariableConfiguration createNewVariableConfiguration(string variableName, VariableType variableType, bool persisted,
                                                        object variableValue, string comment)
        {
            IVariableConfiguration variableConfiguration = createVariableConfiguration(null,variableName, variableType,
                                                                                     persisted,variableValue,comment);
            return variableConfiguration;
        }

        public IVariableConfiguration createNewEmptyVariableConfiguration(string variableName, VariableType variableType,
                                                             bool persisted, string comment)
        {
            IVariableConfiguration variableConfiguration = createVariableConfiguration(null,variableName, variableType,
                                                                                     persisted, Task.UnconfiguredVariable,comment);
            return variableConfiguration;
        }

        #endregion

        #region Execution Time

        public Dictionary<string,IVariable> buildVariables(ITask ownerTask, List<IVariableConfiguration> variableConfigurations)
        {
            Dictionary<string, IVariable> variables = new Dictionary<string, IVariable>();
            foreach (IVariableConfiguration variableConfiguration in variableConfigurations)
            {
                variables.Add(variableConfiguration.VariableName,buildVariable(ownerTask, variableConfiguration));
            }
            return variables;
        }

        private IVariable buildDateTimeVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            if (variableConfiguration.isUnconfigured())
            {
                    // Unconfigured Variable
                    variable = new DateTimeVariable(this, ownerTask, variableConfiguration.VariableName, (DateTime?) null,
                                                    variableConfiguration.Persisted);
                }
                else
                {
                    variable = new DateTimeVariable(this, ownerTask, variableConfiguration.VariableName,
                                                    (DateTime?) variableConfiguration.Value,
                                                    variableConfiguration.Persisted);
                }
                if (variableConfiguration.Persisted)
                {
                    registerPersistedVariable(variable);
                }
            return variable;
        }

        private IVariable buildListOfStringVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            if (variableConfiguration.isUnconfigured())
            {
                    // Unconfigured Variable
                    variable = new ListOfStringVariable(this, ownerTask, variableConfiguration.VariableName,
                                                        null,
                                                        variableConfiguration.Persisted);
                }
                else
                {
                    variable = new ListOfStringVariable(this, ownerTask, variableConfiguration.VariableName,
                                                        variableConfiguration.Value as List<string>,
                                                        variableConfiguration.Persisted);
                }
                if (variableConfiguration.Persisted)
                {
                    registerPersistedVariable(variable);
                }
            return variable;
        }

        private IVariable buildStringVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            if (variableConfiguration.isUnconfigured())
            {
                    // Unconfigured Variable
                    variable = new StringVariable(this, ownerTask, variableConfiguration.VariableName,
                                                  variableConfiguration.Value as string,
                                                  variableConfiguration.Persisted);
                }
                else
                {
                    variable = new StringVariable(this, ownerTask, variableConfiguration.VariableName,
                                                  variableConfiguration.Value as string,
                                                  variableConfiguration.Persisted);
                }
                if (variableConfiguration.Persisted)
                {
                    registerPersistedVariable(variable);
                }
            return variable;
        }

        private IVariable buildIntegerVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            if (variableConfiguration.isUnconfigured())
            {
                    // Unconfigured Variable
                    variable = new IntegerVariable(this, ownerTask, variableConfiguration.VariableName,
                                                   null,
                                                   variableConfiguration.Persisted);
                }
                else
                {
                    variable = new IntegerVariable(this, ownerTask, variableConfiguration.VariableName,
                                                   (int?) variableConfiguration.Value,
                                                   variableConfiguration.Persisted);
                }
                if (variableConfiguration.Persisted)
                {
                    registerPersistedVariable(variable);
                }
            return variable;
        }

        private IVariable buildBooleanVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            if (variableConfiguration.isUnconfigured())
            {
                // Unconfigured Variable
                variable = new BooleanVariable(this, ownerTask, variableConfiguration.VariableName,
                                               null,
                                               variableConfiguration.Persisted);
            }
            else
            {
                variable = new BooleanVariable(this, ownerTask, variableConfiguration.VariableName,
                                               (bool?)variableConfiguration.Value,
                                               variableConfiguration.Persisted);
            }
            if (variableConfiguration.Persisted)
            {
                registerPersistedVariable(variable);
            }
            return variable;
        }

        private IVariable buildBooleanPlaceHolderVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            variable = new BooleanPHVariable(ownerTask, variableConfiguration.VariableName, variableConfiguration.Value as string);
            return variable;
        }

        
        
        private IVariable buildIntegerPlaceHolderVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
                variable = new IntegerPHVariable(ownerTask,variableConfiguration.VariableName, variableConfiguration.Value as string);
            return variable;
        }
        private IVariable buildStringPlaceHolderVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            variable = new StringPHVariable(ownerTask, variableConfiguration.VariableName, variableConfiguration.Value as string);
            return variable;
        }

        private IVariable buildListOfStringPlaceHolderVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            variable = new ListOfStringPHVariable(ownerTask, variableConfiguration.VariableName, variableConfiguration.Value as string);
            return variable;
        }

        private IVariable buildDateTimePlaceHolderVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable variable;
            variable = new DateTimePHVariable(ownerTask, variableConfiguration.VariableName, variableConfiguration.Value as string);
            return variable;
        }


        public void setVariables(ITask ownerTask, Dictionary<string, IVariable> variables,
                                 Dictionary<string, IVariableConfiguration> variableConfigurations)
        {
            foreach (IVariableConfiguration variableConfiguration in variableConfigurations.Values)
            {
                setVariable(ownerTask, variables, variableConfiguration);
            }
        }

        private void setVariable(ITask ownerTask, Dictionary<string, IVariable> variables,
                                 IVariableConfiguration variableConfiguration)
        {
            if (!variables.ContainsKey(variableConfiguration.VariableName))
            {
                createVariable(ownerTask, variables, variableConfiguration);
            }
            //else
            //{
            //    foreach (IVariable variable in variables.Values)
            //    {
            //        if (variable.VariableName.Equals(variableConfiguration.VariableName) &&
            //            variable.Type == variableConfiguration.VariableType &&
            //            (!variable.Persisted || (variable.Persisted && !variable.HasValue)))
            //        {
            //            switch (variable.Type)
            //            {
            //                case VariableType.Integer:
            //                    if (variable is IIntegerVariable)
            //                    {
            //                        IIntegerVariable integerVariable = variable as IIntegerVariable;
            //                        integerVariable.Value = variableConfiguration.Value as int?;
            //                    }
            //                    break;
            //                case VariableType.String:
            //                    if (variable is IStringVariable)
            //                    {
            //                        IStringVariable stringVariable = variable as IStringVariable;
            //                        stringVariable.Value = variableConfiguration.Value as string;
            //                    }
            //                    break;
            //                case VariableType.ListOfString:
            //                    if (variable is IListOfStringVariable)
            //                    {
            //                        IListOfStringVariable listOfStringVariable = variable as IListOfStringVariable;
            //                        List<string> strings = variableConfiguration.Value as List<string>;
            //                        if (strings != null)
            //                        {
            //                            foreach (string s in strings)
            //                            {
            //                                listOfStringVariable.addString(s);
            //                            }
            //                        }
            //                    }
            //                    break;
            //                case VariableType.DateTime:
            //                    if (variable is IDateTimeVariable)
            //                    {
            //                        IDateTimeVariable dateTimeVariable = variable as IDateTimeVariable;
            //                        dateTimeVariable.Value = variableConfiguration.Value as DateTime?;
            //                    }
            //                    break;
            //                default:
            //                    throw new TaskConfigurationException("VariableFactory", "setVariable", "Unknown",
            //                                                         string.Format("VariableType {0} does not exist or cannot be set",
            //                                                                       variable.Type.ToString()));
            //            }
            //        }
            //    }
            //}
        }

        private void createVariable(ITask ownerTask, Dictionary<string,IVariable> variables,
                                    IVariableConfiguration variableConfiguration)
        {
            variables.Add(variableConfiguration.VariableName,buildVariable(ownerTask, variableConfiguration));
        }

        private IVariable buildVariable(ITask ownerTask, IVariableConfiguration variableConfiguration)
        {
            IVariable returnVariable = null;
            switch (variableConfiguration.VariableType)
            {
                case VariableType.Integer:
                    returnVariable = buildIntegerVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.Boolean:
                    returnVariable = buildBooleanVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.String:
                    returnVariable = buildStringVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.ListOfString:
                    returnVariable = buildListOfStringVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.DateTime:
                    returnVariable = buildDateTimeVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.IntegerPlaceHolder:
                    returnVariable = buildIntegerPlaceHolderVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.BooleanPlaceHolder:
                    returnVariable = buildBooleanPlaceHolderVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.StringPlaceHolder:
                    returnVariable = buildStringPlaceHolderVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.ListOfStringPlaceHolder:
                    returnVariable = buildListOfStringPlaceHolderVariable(ownerTask, variableConfiguration);
                    break;
                case VariableType.DateTimePlaceHolder:
                    returnVariable = buildDateTimePlaceHolderVariable(ownerTask, variableConfiguration);
                    break;
                default:
                    throw new FactoryException(GetType().Name, "buildVariables",
                                                                "VariableType not supported: " +
                                                                variableConfiguration.VariableType.ToString());
            }
            return returnVariable;
        }

        #endregion

        #region Creation Time

        public IVariableConfiguration copyVariableConfiguration(ITaskConfiguration owningTaskConfiguration,IVariableConfiguration variableConfiguration, bool removePersistedValues)
        {
            IVariableConfiguration copyVariableConfiguration;
            if(removePersistedValues)
            {
                copyVariableConfiguration =
                    createVariableConfiguration(owningTaskConfiguration,variableConfiguration.VariableName, variableConfiguration.VariableType, false,
                                                copyVariableConfigurationValue(variableConfiguration), variableConfiguration.Comment);
                if(variableConfiguration.Persisted && getVariablePersister().checkForPersistedValue(variableConfiguration.FullName))
                {
                    getVariablePersister().gettingPersistedVariable(copyVariableConfiguration);
                }
            }
            else
            {
                copyVariableConfiguration =
                    createVariableConfiguration(owningTaskConfiguration,variableConfiguration.VariableName, variableConfiguration.VariableType,
                                                variableConfiguration.Persisted,
                                                variableConfiguration, variableConfiguration.Comment);

            }
            return copyVariableConfiguration;
        }

        private object copyVariableConfigurationValue(IVariableConfiguration variableConfiguration)
        {
            return variableConfiguration.Value;
        }

        private IVariableConfiguration createVariableConfiguration(ITaskConfiguration owningTaskConfiguration, string variableName, VariableType variableType, bool persisted, string comment)
        {
            return new VariableConfiguration(this,owningTaskConfiguration, variableName, variableType, persisted,comment);
        }

        private IVariableConfiguration createVariableConfiguration(ITaskConfiguration owningTaskConfiguration, string variableName, VariableType variableType, bool persisted, object variableValue, string comment)
        {
            IVariableConfiguration variableConfiguration = new VariableConfiguration(this, owningTaskConfiguration, variableName, variableType, persisted,comment);
            variableConfiguration.Value = variableValue;
            return variableConfiguration;
        }

        public VariableType getPlaceHolderVariableType(VariableType originalVariableType)
        {
            VariableType variableType = VariableType.Unknown;
            switch (originalVariableType)
            {
                case VariableType.DateTime:
                    variableType = VariableType.DateTimePlaceHolder;
                    break;
                case VariableType.String:
                    variableType = VariableType.StringPlaceHolder;
                    break;
                case VariableType.ListOfString:
                    variableType = VariableType.ListOfStringPlaceHolder;
                    break;
                case VariableType.Integer:
                    variableType = VariableType.IntegerPlaceHolder;
                    break;
                case VariableType.Boolean:
                    variableType = VariableType.BooleanPlaceHolder;
                    break;
            }
            return variableType;
        }

        public Dictionary<string, IVariableConfiguration> buildVariableConfigurations(string jobScheduleName, ITaskConfiguration owningTaskConfiguration,
                                                                        XmlNodeList xmlNodeList)
        {
            Dictionary<string, IVariableConfiguration> variableConfigurations = new Dictionary<string, IVariableConfiguration>();
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.Name == BuildSchedule.Variable)
                {
                    IVariableConfiguration variableConfiguration = buildVariableConfiguration(jobScheduleName,
                                                                                              owningTaskConfiguration,
                                                                                              xmlNode);
                    variableConfigurations.Add(variableConfiguration.VariableName, variableConfiguration);
                }
            }
            return variableConfigurations;
        }

        private IVariableConfiguration buildVariableConfiguration(string jobScheduleName, ITaskConfiguration owningTaskConfiguration, XmlNode xmlNode)
        {
            string variableName = "";
            VariableType variableType = VariableType.Unknown;
            bool persisted = false;
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                if (xmlAttribute.Name == BuildSchedule.VariableName)
                {
                    if (FactoryHelper.variableIsUnconfigured(xmlAttribute.Value))
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildVariableConfiguration", owningTaskConfiguration.TaskName, string.Format("Variable name has not been configured in job {0}", jobScheduleName));
                    }
                    else
                    {
                        variableName = xmlAttribute.Value;
                    }
                }
                if (xmlAttribute.Name.Equals(BuildSchedule.VariableType))
                {
                    variableType = buildVariableTypeFromName(jobScheduleName, owningTaskConfiguration.TaskName, xmlAttribute.Value);
                }
                if (xmlAttribute.Name.Equals(BuildSchedule.Persisted))
                {
                    if (!bool.TryParse(xmlAttribute.Value, out persisted))
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildVariableConfiguration", owningTaskConfiguration.TaskName,

                                                                    string.Format("Could not parse persisted in {0}", jobScheduleName));
                    }
                }
            }
            IVariableConfiguration variableConfiguration = createVariableConfiguration(owningTaskConfiguration,variableName, variableType,
                                                                                     persisted,"");
            if (variableType != VariableType.Unknown)
            {
                variableConfiguration.Value = buildValuesFromVariableType(owningTaskConfiguration.TaskName, variableType, xmlNode.ChildNodes);
            }
            return variableConfiguration;
        }

        private object buildValuesFromVariableType( string owningTaskName, VariableType variableType,
                                                   XmlNodeList xmlNodeList)
        {
            string stringValue = "";
            switch (variableType)
            {
                case VariableType.Integer:
                    int value;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if (xmlNode.Name.Equals(BuildSchedule.VariableValue))
                        {
                            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                            {
                                if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                                {
                                    stringValue = xmlAttribute.Value;
                                }
                            }
                        }
                    }

                    if (stringValue.Equals(Task.UnconfiguredVariable))
                    {
                        return null;
                    }
                    else if (xmlNodeList.Count > 0 && int.TryParse(stringValue, out value))
                    {
                        return value as int?;
                    }
                    else
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildValuesFromVariableType", owningTaskName,
                                                                    "Could not parse int");
                    }
                case VariableType.Boolean:
                    bool boolValue;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if (xmlNode.Name.Equals(BuildSchedule.VariableValue))
                        {
                            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                            {
                                if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                                {
                                    stringValue = xmlAttribute.Value;
                                }
                            }
                        }
                    }

                    if (stringValue.Equals(Task.UnconfiguredVariable))
                    {
                        return null;
                    }
                    else if (xmlNodeList.Count > 0 && bool.TryParse(stringValue, out boolValue))
                    {
                        return boolValue as bool?;
                    }
                    else
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildValuesFromVariableType", owningTaskName,
                                                                    "Could not parse boolean");
                    }
                case VariableType.String:
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if (xmlNode.Name.Equals(BuildSchedule.VariableValue))
                        {
                            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                            {
                                if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                                {
                                    stringValue = xmlAttribute.Value;
                                }
                            }
                        }
                    }

                    if (stringValue.Equals(Task.UnconfiguredVariable))
                    {
                        return null;
                    }
                    else if (xmlNodeList.Count > 0)
                    {
                        return stringValue;
                    }
                    else
                    {
                        throw new TaskConfigurationException(GetType().Name,
                                                                    "buildValuesFromVariableType",owningTaskName,
                                                                    "No children on node");
                    }
                case VariableType.ListOfString:
                    List<string> strings = new List<string>();
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if (xmlNode.Name.Equals(BuildSchedule.VariableValue))
                        {
                            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                            {
                                if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                                {
                                    stringValue = xmlAttribute.Value;
                                    strings.Add(xmlAttribute.Value);
                                }
                            }
                        }
                    }

                    if (stringValue.Equals(Task.UnconfiguredVariable))
                    {
                        return null;
                    }
                    else
                    {
                        return strings;
                    }
                case VariableType.DateTime:
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if (xmlNode.Name.Equals(BuildSchedule.VariableValue))
                        {
                            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                            {
                                if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                                {
                                    stringValue = xmlAttribute.Value;
                                }
                            }
                        }
                    }

                    if (stringValue.Equals(Task.UnconfiguredVariable))
                    {
                        return null;
                    }
                    else
                    {
                        return FactoryHelper.getDateTime(stringValue, true);
                    }
                default:
                    throw new FactoryException(GetType().Name, "buildValuesFromVariableType",
                                                                "VariableType " + variableType.ToString() +
                                                                " cannot be handled");
            }
        }

        private VariableType buildVariableTypeFromName(string jobScheduleName, string owningTaskName, string variableTypeName)
        {
            if (variableTypeName.Equals(VariableType.Integer.ToString()))
            {
                return VariableType.Integer;
            }
            else if (variableTypeName.Equals(VariableType.Boolean.ToString()))
            {
                return VariableType.Boolean;
            }
            else if (variableTypeName.Equals(VariableType.String.ToString()))
            {
                return VariableType.String;
            }
            else if (variableTypeName.Equals(VariableType.ListOfString.ToString()))
            {
                return VariableType.ListOfString;
            }
            else if (variableTypeName.Equals(VariableType.DateTime.ToString()))
            {
                return VariableType.DateTime;
            }
            else if (variableTypeName.Equals(VariableType.IntegerPlaceHolder.ToString()))
            {
                return VariableType.IntegerPlaceHolder;
            }
            else if (variableTypeName.Equals(VariableType.BooleanPlaceHolder.ToString()))
            {
                return VariableType.BooleanPlaceHolder;
            }
            else if (variableTypeName.Equals(VariableType.StringPlaceHolder.ToString()))
            {
                return VariableType.StringPlaceHolder;
            }
            else if (variableTypeName.Equals(VariableType.ListOfStringPlaceHolder.ToString()))
            {
                return VariableType.ListOfStringPlaceHolder;
            }
            else if (variableTypeName.Equals(VariableType.DateTimePlaceHolder.ToString()))
            {
                return VariableType.DateTimePlaceHolder;
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "buildVariableTypeFromName",owningTaskName,
                                                            string.Format("VariableType {0} does not exist. Error in job {1}", variableTypeName, jobScheduleName));
            }
        }

        #endregion
    }
}