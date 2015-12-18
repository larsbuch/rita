using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface IVariableFactory
    {
        IMainFactory MainFactory { get; set; }
        Dictionary<string,IVariable> buildVariables(ITask ownerTask,List<IVariableConfiguration> variableConfigurations);
        void setVariables(ITask ownerTask, Dictionary<string, IVariable> variables, Dictionary<string, IVariableConfiguration> variableConfigurations);

        Dictionary<string, IVariableConfiguration> buildVariableConfigurations(string jobScheduleName,
                                                                 ITaskConfiguration owningTaskConfiguration,
                                                                 XmlNodeList xmlNodeList);
        IVariableConfiguration createNewVariableConfiguration(string variableName, VariableType variableType, bool persisted, object value, string comment);
        IVariableConfiguration createNewEmptyVariableConfiguration(string variableName, VariableType variableType, bool persisted, string comment);
        void settingPersistedVariable(IVariable variable);
        IVariableConfiguration copyVariableConfiguration(ITaskConfiguration owningTaskConfiguration, IVariableConfiguration variableConfiguration, bool removePersistedValues);
        VariableType getPlaceHolderVariableType(VariableType originalVariableType);
    }
}
