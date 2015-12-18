using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;

namespace RukisIntegrationTaskhandlerExtension
{
    public class TaskConfiguration : ITaskConfiguration
    {
        private ITaskConfiguration _owningTaskConfiguration;
        private ITaskFactory _taskFactory;
        private string _jobScheduleName;

        public TaskConfiguration(ITaskFactory taskFactory,string jobScheduleName , ITaskConfiguration owningTaskconfiguration)
        {
            VariableConfigurations = new Dictionary<string, IVariableConfiguration>();
            ChildTaskConfigurations = new List<ITaskConfiguration>();
            _owningTaskConfiguration = owningTaskconfiguration;
            _taskFactory = taskFactory;
            _jobScheduleName = jobScheduleName;
        }

        public string FullName
        {
            get
            {
                if (_owningTaskConfiguration == null)
                {
                    if (_jobScheduleName == null || _jobScheduleName.Equals(string.Empty))
                    {
                        return TaskName;
                    }
                    else
                    {
                        return _jobScheduleName + Task.TaskChildNameSeparator + TaskName;
                    }
                }
                else
                {
                    return _owningTaskConfiguration.FullName + Task.TaskChildNameSeparator + TaskName;
                }
            }
        }

        public string TaskName { get; set; }
        public string TaskClassName { get; set; }
        public Dictionary<string, IVariableConfiguration> VariableConfigurations { get; set; }
        public List<ITaskConfiguration> ChildTaskConfigurations { get; set; }
        public ITaskConfiguration getCopy(ITaskConfiguration owningTaskConfiguration, bool removePersistedValues)
        {
            ITaskConfiguration taskConfiguration = _taskFactory.createTaskConfigurationWithEmptySchedule(_jobScheduleName,owningTaskConfiguration);
            taskConfiguration.TaskName = TaskName;
            taskConfiguration.TaskClassName = TaskClassName;

            foreach (ITaskConfiguration childTaskConfiguration in ChildTaskConfigurations)
            {
                taskConfiguration.ChildTaskConfigurations.Add(childTaskConfiguration.getCopy(taskConfiguration,removePersistedValues));
            }
            foreach (IVariableConfiguration variableConfiguration in VariableConfigurations.Values)
            {
                taskConfiguration.VariableConfigurations.Add(variableConfiguration.VariableName,variableConfiguration.getCopy(taskConfiguration,removePersistedValues));
            }

            return taskConfiguration;
        }

        public void setOwningTaskConfiguration(ITaskConfiguration owningTaskConfiguration)
        {
            _owningTaskConfiguration = owningTaskConfiguration;
        }

        public void setJobScheduleName(string jobScheduleName)
        {
            _jobScheduleName = jobScheduleName;
        }
    }
}