using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface ITask
    {
        void executeTask(ITaskExecutionContext taskExecutionContext);
        string TaskName { get; }
        string TaskFullName { get; }
        ITask ParentTask { get; }
        string TaskClassName { get; }
        void setVariableConfigurations(Dictionary<string, IVariableConfiguration> variableConfigurations);
        Dictionary<string, IVariableConfiguration> getVariableConfigurations();
        void addChildTasks(List<ITask> childTasks);
        Dictionary<string, ITask> getChildTasks();
        void setVariableConfiguration(IVariableConfiguration variableConfiguration);
        IVariableConfiguration getVariableConfiguration(string variableName);
        void resetTask();
        void initializeTask();
        bool TaskInitialized { get;}
        IVariable getVariable(string variableName);
        IJobRunner OwningJobRunner { get; }
        bool hasVariable(string variableName);
        void configure(ITask parentTask, string taskName, IVariableFactory variableFactory, ITaskFactory taskFactory, IJobRunner jobRunner);
    }
}
