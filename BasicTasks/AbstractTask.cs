using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace BasicTasks
{
    public abstract class AbstractTask : ITask
    {

        #region Public

        public AbstractTask()
        {
            _childTasks = new Dictionary<string, ITask>();
            _variableConfiguration = new Dictionary<string, IVariableConfiguration>();
            _variables = new Dictionary<string, IVariable>();
            TaskInitialized = false;
        }

        public void configure(ITask parentTask, string taskName, IVariableFactory variableFactory, ITaskFactory taskFactory, IJobRunner jobRunner)
    {
                    ParentTask = parentTask;
            TaskName = taskName;
            VariableFactory = variableFactory;
            TaskFactory = taskFactory;
            OwningJobRunner = jobRunner;

    }


        protected IVariableFactory VariableFactory { get; set; }
        protected ITaskFactory TaskFactory { get; set; }
        public IJobRunner OwningJobRunner { get; protected set; }

        public bool TaskInitialized { get; protected set; }

        public IVariable getVariable(string variableName)
        {
            if (_variables.ContainsKey(variableName))
            {
                return _variables[variableName];
            }
            else
            {
                throw new TaskExecutionException(TaskName, "getVariable", string.Format("Could not get variable {0}", variableName));
            }
        }

        public void executeTask(ITaskExecutionContext taskExecutionContext)
        {
            try
            {
                if (taskExecutionContext == null)
                {
                    throw new TaskExecutionException(TaskName, "executeTask", "taskExecutionContext was null");
                }
                if (taskExecutionContext.StartAtTask == TaskName)
                {
                    resetTask();
                    taskExecutionContext.StartTaskFound = true;
                }
                if (taskExecutionContext.StartTaskFound)
                {
                    taskExecutionContext.setCurrentlyExecutingTask(TaskName);
                }

                // Set variables
                setVariables(getVariableConfigurations());

                // Check that execution should happen
                if (taskExecutionContext.StartTaskFound)
                {
                    preExecuteTaskInternally(taskExecutionContext);
                    executeTaskInternally(taskExecutionContext);
                    postExecuteTaskInternally(taskExecutionContext);
                }
            }
            catch (TaskExecutionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TaskExecutionException(GetType().Name,"executeTask", "",e);
            }
        }

        public string TaskName
        {
            get
            {
                if (_name == null)
                {
                    return GetType().Name;
                }
                else
                {
                        return _name;
                    }
            }
            protected set { _name = value; }
        }

        public string TaskFullName
        {
            get
            {
                if (_parentTask == null)
                {
                    if (OwningJobRunner == null)
                    {
                        return _name;
                    }
                    else
                    {
                        return OwningJobRunner.JobName + Task.TaskChildNameSeparator + _name;
                    }
                }
                else
                {
                    return _parentTask.TaskName + Task.TaskChildNameSeparator + _name;
                }
            }
        }

        public string TaskClassName
        {
            get { return GetType().FullName; }
        }

        private void setVariables(Dictionary<string,IVariableConfiguration> variableConfigurations)
        {
            VariableFactory.setVariables(this, _variables, variableConfigurations);
        }

        public void setVariableConfigurations(Dictionary<string,IVariableConfiguration> variableConfigurations)
        {
            foreach (IVariableConfiguration variableConfiguration in variableConfigurations.Values)
            {
                setVariableConfiguration(variableConfiguration);
            }
        }

        public Dictionary<string, IVariable> getVariables()
        {
            return _variables;
        }

        public void addChildTasks(List<ITask> childTasks)
        {
            if (childTasks != null && childTasks.Count != 0)
            {
                foreach (ITask childTask in childTasks)
                {
                    addChildTask(childTask);
                }
            }
        }

        public void addChildTask(ITask childTask)
        {
            if (childTask != null)
            {
                if(!_childTasks.ContainsKey(childTask.TaskName))
                {
                    // Add new child task
                    _childTasks.Add(childTask.TaskName, childTask);
                }
                else
                {
                    // Replace existing
                    _childTasks[childTask.TaskName] = childTask;
                }
            }
        }

        public Dictionary<string, ITask> getChildTasks()
        {
            return _childTasks;
        }

        public bool hasVariable(string variableName)
        {
            return _variableConfiguration.ContainsKey(variableName);
        }

        public void setVariableConfiguration(IVariableConfiguration variableConfiguration)
        {
            if(_variableConfiguration.ContainsKey(variableConfiguration.VariableName))
            {
                _variableConfiguration[variableConfiguration.VariableName] = variableConfiguration;
            }
            else
            {
                _variableConfiguration.Add(variableConfiguration.VariableName,variableConfiguration);
            }
        }

        public IVariableConfiguration getVariableConfiguration(string variableName)
        {
            if (_variableConfiguration.ContainsKey(variableName))
            {
                return _variableConfiguration[variableName];
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "getVariableConfiguration", TaskName, "Failed to get from dictionary");
            }
        }

        public ITask ParentTask
        {
            get { return _parentTask; }
            protected set { _parentTask = value; }
        }

        public Dictionary<string,IVariableConfiguration> getVariableConfigurations()
        {
            return _variableConfiguration;
        }

        public virtual void initializeTask()
        {
            //Do nothing except set initialized
            TaskInitialized = true;
        }

        #endregion

        #region Protected & Private

        private ITask _parentTask;
        private Dictionary<string, ITask> _childTasks;
        private Dictionary<string, IVariable> _variables;
        private string _name;
        private Dictionary<string, IVariableConfiguration> _variableConfiguration;
        protected static readonly string DirectoryEnd = Misc.DirectoryEnd;

        protected Dictionary<string, ITask> ChildTasks
        {
            get
            {
                return _childTasks;
            }
        }

        protected void addVariableConfiguration(IVariableConfiguration variableConfiguration)
        {
            _variableConfiguration.Add(variableConfiguration.VariableName, variableConfiguration);
        }

        protected void executeChildTasks(Dictionary<string,ITask> subtasks, ITaskExecutionContext taskExecutionContext)
        {
            if (taskExecutionContext == null)
            {
                throw new TaskConfigurationException(GetType().Name, "executeSubtasks", TaskName,
                                                     "taskExecutionContext was null");
            }
            if (subtasks == null)
            {
                throw new TaskConfigurationException(GetType().Name, "executeSubtasks", TaskName, "subtasks was null");
            }
            foreach (ITask childTask in subtasks.Values)
            {
                childTask.executeTask(taskExecutionContext);
            }
        }

        #endregion

        #region Foreign Constructors

        protected void createNewEmptyVariable(string variableName, VariableType variableType, bool persisted, string comment)
        {
            addVariableConfiguration(VariableFactory.createNewEmptyVariableConfiguration(variableName, variableType, persisted, comment));
        }

        protected void createNewVariable(string variableName, VariableType variableType, bool persisted, object value, string comment)
        {
            addVariableConfiguration(VariableFactory.createNewVariableConfiguration(variableName, variableType, persisted, value, comment));
        }

        protected void createNewChildTask(string taskName, string taskClassName)
        {
            ITask task = TaskFactory.createNewChildTask(taskName, taskClassName, this);
            addChildTask(task);
        }

        protected void replaceVariableWithPlaceHolder(string childTaskName,string variableNameToReplace, string parentVariableName)
        {
            if(OwningJobRunner.CanExecute)
            {
                ITask childTask;
                if(getChildTasks().ContainsKey(childTaskName))
                {
                    childTask = getChildTasks()[childTaskName];
                }
                else
                {
                    throw new TaskConfigurationException(GetType().Name, "replaceVariableWithPlaceholder",TaskName, string.Format("Child task {0} does not exist",
                                                                       childTaskName));
                }
                if (!_variableConfiguration.ContainsKey(parentVariableName))
                {
                    throw new TaskConfigurationException(GetType().Name, "replaceVariableWithPlaceHolder", TaskName,
                                                         string.Format("ParentVariable {0} does not exist",
                                                                       parentVariableName));
                }
                IVariableConfiguration variableToBeReplaced = childTask.getVariableConfiguration(variableNameToReplace);
                IVariableConfiguration parentVariableConfiguration = getVariableConfiguration(parentVariableName);


                if (parentVariableConfiguration.VariableType != variableToBeReplaced.VariableType)
                {
                    throw new TaskConfigurationException(GetType().Name, "replaceVariableWithPlaceHolder", TaskName,
                                                         string.Format("Type mismatch {0} != {1}",
                                                                       parentVariableConfiguration.VariableType.ToString(),
                                                                       variableToBeReplaced.VariableType.ToString()));
                }

                variableToBeReplaced.VariableType = VariableFactory.getPlaceHolderVariableType(variableToBeReplaced.VariableType);
                variableToBeReplaced.Persisted = false;
                variableToBeReplaced.Value = parentVariableName;
            }
        }

        protected void setChildVariable(string childTaskName, string variableName, VariableType variableType, object variableValue, string comment)
        {
            ITask childTask;
            if (getChildTasks().ContainsKey(childTaskName))
            {
                childTask = getChildTasks()[childTaskName];
            }
            else
            {
                throw new TaskConfigurationException(GetType().Name, "setChildVariable", TaskName, string.Format("Child task {0} does not exist",
                                                                   childTaskName));
            }

            if (childTask.hasVariable(variableName))
            {
                IVariableConfiguration variableConfiguration = childTask.getVariableConfiguration(variableName);
                variableConfiguration.Value = variableValue;
            }
            else
            {
                childTask.setVariableConfiguration(VariableFactory.createNewVariableConfiguration(variableName, variableType, false, variableValue, comment));
            }
        }

        protected void setVariable(string variableName, object variableValue)
        {
            if(hasVariable(variableName))
            {
                getVariableConfiguration(variableName).Value = variableValue;
            }
        }

        #endregion

        #region AbstractMethods

        protected abstract void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext);
        protected abstract void executeTaskInternally(ITaskExecutionContext taskExecutionContext);
        protected abstract void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext);
        public abstract void resetTask();

        #endregion
    }
}