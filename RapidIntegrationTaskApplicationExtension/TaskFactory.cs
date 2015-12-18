using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;

namespace RukisIntegrationTaskhandlerExtension
{
    public class TaskFactory : ITaskFactory
    {
        #region Initializing Part

        /* Throws Factory Exception */

        private static Dictionary<string, LoadedTask> _taskList;
        private static Dictionary<string, ITaskConfiguration> _taskConfigurationList;

        public TaskFactory()
        {
            _taskList = new Dictionary<string, LoadedTask>();
        }

        public IMainFactory MainFactory { get; set; }

        public void loadFactory()
        {
            string fullDirectoryName = GlobalVariables.getServiceLocation() + Misc.TaskFolderPlacement;

            string[] files = Directory.GetFiles(fullDirectoryName, Misc.TaskFilter);
            foreach (string file in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    System.Type[] types = asm.GetTypes();

                    foreach (System.Type type in types)
                    {
                        if (type.GetInterface("I" + Misc.Task) != null && type.IsAbstract == false)
                        {
                            _taskList.Add(type.FullName, new LoadedTask(type.FullName, asm));
                        }
                    }
                }
                catch (RukisIntegrationTaskhandlerException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new FactoryException(GetType().Name, "loadFactory",
                                               String.Format("Could not import tasks from {0} task assembly", file), e);
                }
            }
            try
            {
                _taskConfigurationList = createTaskConfigurationList();
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FactoryException(GetType().Name, "loadFactory",
                                           String.Format("Could not create task configurations"), e);
            }
        }

        public Dictionary<string, ITaskConfiguration> createTaskConfigurationList()
        {
            Dictionary<string, ITaskConfiguration> taskConfigurations = new Dictionary<string, ITaskConfiguration>();
            IJobRunner jobRunner = MainFactory.JobRunnerFactory.getDummyJobRunner();
            IJobSchedule jobSchedule = MainFactory.JobScheduleFactory.createNewJobSchedule();
            foreach (string taskClassName in _taskList.Keys)
            {
                ITask task = createTaskFromName(jobRunner, null, Task.UnconfiguredTaskName, taskClassName);
                task.initializeTask();
                // not needed as task is not runnable
                //task.setVariables(MainFactory, task.getVariableConfigurations());
                ITaskConfiguration taskConfiguration = buildTaskConfiguration(jobSchedule.getJobName(),task,null);
                taskConfigurations.Add(taskClassName, taskConfiguration);
            }
            return taskConfigurations;
        }

        public Dictionary<string, ITaskConfiguration> getTaskConfigurationList()
        {
            return _taskConfigurationList;
        }

        public Dictionary<string, ITaskConfiguration> getDefaultTaskConfigurationList()
        {
            return buildDefaultTaskConfigurationList("Default Creation Schedule",_taskConfigurationList);
        }

        public ITaskConfiguration getDefaultTaskConfiguration(string taskClassName)
        {
            if(!_taskConfigurationList.ContainsKey(taskClassName))
            {
                throw new FactoryException(GetType().Name, "getDefaultTaskConfiguration",
                                           "Task class does not exist: " + taskClassName);
            }
            return _taskConfigurationList[taskClassName].getCopy(null,false);
        }

        public ITaskConfiguration createTaskConfigurationWithEmptySchedule(string jobScheduleName, ITaskConfiguration owningTaskConfiguration)
        {
            return createTaskConfiguration(jobScheduleName, owningTaskConfiguration);
        }

        public ITaskConfiguration createTaskConfiguration(string jobScheduleName,ITaskConfiguration owningTaskConfiguration)
        {
            return new TaskConfiguration(this, jobScheduleName, owningTaskConfiguration);
        }


        private ITaskConfiguration buildTaskConfiguration(string jobScheduleName, ITask task, ITaskConfiguration owningTaskConfiguration)
        {
            ITaskConfiguration taskConfiguration = createTaskConfiguration(jobScheduleName, owningTaskConfiguration);
            
            // Get Name
            taskConfiguration.TaskName = task.TaskName;
            taskConfiguration.TaskClassName = task.TaskClassName;

            // Get children
            foreach (ITask childTask in task.getChildTasks().Values)
            {
                taskConfiguration.ChildTaskConfigurations.Add(buildTaskConfiguration(jobScheduleName, childTask, taskConfiguration));
            }

            // Get variables
            foreach (IVariableConfiguration variableConfiguration in task.getVariableConfigurations().Values)
            {
                taskConfiguration.VariableConfigurations.Add(variableConfiguration.VariableName, variableConfiguration);
            }
            return taskConfiguration;
        }

        private Dictionary<string, ITaskConfiguration> buildDefaultTaskConfigurationList(string jobScheduleName,
            Dictionary<string, ITaskConfiguration> taskConfigurations)
        {
            Dictionary<string, ITaskConfiguration> defaultTaskConfigurations =
                new Dictionary<string, ITaskConfiguration>();
            foreach (KeyValuePair<string, ITaskConfiguration> pair in taskConfigurations)
            {
                defaultTaskConfigurations.Add(pair.Key, buildDefaultTaskConfiguration(jobScheduleName, null, pair.Value));
            }

            return defaultTaskConfigurations;
        }

        private ITaskConfiguration buildDefaultTaskConfiguration(string jobScheduleName, ITaskConfiguration owningTaskConfiguration, ITaskConfiguration taskConfiguration)
        {
            ITaskConfiguration defaultTaskConfiguration = createTaskConfiguration(jobScheduleName, owningTaskConfiguration);

            // Get Name
            defaultTaskConfiguration.TaskName = taskConfiguration.TaskName;
            defaultTaskConfiguration.TaskClassName = taskConfiguration.TaskClassName;

            // Get children
            foreach (ITaskConfiguration childTaskConfiguration in taskConfiguration.ChildTaskConfigurations)
            {
                ITaskConfiguration defaultChildTaskConfiguration = buildDefaultTaskConfiguration(jobScheduleName, defaultTaskConfiguration, childTaskConfiguration);

                // Add only tasks with either unconfigured variables or children with unconfigured variables
                if (defaultChildTaskConfiguration.VariableConfigurations.Count >= 1 ||
                    defaultChildTaskConfiguration.ChildTaskConfigurations.Count >= 1)
                {
                    defaultTaskConfiguration.ChildTaskConfigurations.Add(defaultChildTaskConfiguration);
                }
            }

            // Get variables
            foreach (IVariableConfiguration variableConfiguration in taskConfiguration.VariableConfigurations.Values)
            {
                if (FactoryHelper.variableIsUnconfigured(variableConfiguration))
                {
                    IVariableConfiguration defaultVariableConfiguration = variableConfiguration.getCopy(defaultTaskConfiguration,false);
                    defaultTaskConfiguration.VariableConfigurations.Add(defaultVariableConfiguration.VariableName, defaultVariableConfiguration);
                }
            }
            return defaultTaskConfiguration;
        }

        public bool taskClassNameExists(string taskClassName)
        {
            return _taskList.ContainsKey(taskClassName);
        }

        #endregion

        #region Executing Part

        /* Throws TaskExecutionException */


        public ITask buildTask(IJobRunner jobRunner, ITaskConfiguration taskConfiguration)
        {
            return buildTask(jobRunner, taskConfiguration, null);
        }

        public ITask createNewChildTask(string taskName, string taskClassName, ITask parentTask)
        {
            ITask task = createTaskFromName(parentTask.OwningJobRunner, parentTask, taskName, taskClassName);
            task.initializeTask();
            return task;
        }

        private ITask buildTask(IJobRunner jobRunner, ITaskConfiguration taskConfiguration, ITask parentTask)
        {
            if (taskConfiguration == null)
            {
                throw new TaskExecutionException("Unknown in TaskFactory", "buildTask", "taskConfiguration is null");
            }

            try
            {
                ITask task = createTaskFromName(jobRunner, parentTask, taskConfiguration);
                List<ITask> childTasks = new List<ITask>();
                foreach (ITaskConfiguration childTaskConfiguration in taskConfiguration.ChildTaskConfigurations)
                {
                    childTasks.Add(buildTask(jobRunner, childTaskConfiguration, task));
                }
                task.addChildTasks(childTasks);
                if (parentTask != null && parentTask.TaskInitialized)
                {
                    throw new FactoryException(GetType().Name, "CreateTaskFromName", "Parent initialized before child");
                }
                task.initializeTask();
                task.setVariableConfigurations(taskConfiguration.VariableConfigurations);
                return task;
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string message = string.Format("buildTask of {0} failed", taskConfiguration.TaskClassName);
                if (parentTask != null && parentTask.TaskName != null && !parentTask.TaskName.Equals(string.Empty))
                {
                    message = message + string.Format(" with parent {0}", parentTask.TaskName);
                }
                throw new TaskExecutionException(taskConfiguration.TaskName, "buildTask", message, ex);
            }
        }

        #endregion

        #region Configuring Part

        /* Throws TaskConfigurationException */

        public ITaskConfiguration buildGroupTaskConfiguration(string jobScheduleName, XmlNode xmlNode)
        {
            ITaskConfiguration groupTask = createTaskConfiguration(jobScheduleName, null);
            try
            {
                groupTask.TaskName = xmlNode.Name;
                groupTask.TaskClassName = BuildSchedule.GroupTaskName;
                groupTask.ChildTaskConfigurations = buildTaskConfigurations(jobScheduleName, groupTask, xmlNode.ChildNodes);
                return groupTask;
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TaskConfigurationException(GetType().Name, "buildGroupTaskConfiguration",
                                                     BuildSchedule.GroupTaskName,
                                                     string.Format("build task group configuration failed for job {0}",
                                                                   jobScheduleName), e);
            }
        }

        private List<ITaskConfiguration> buildTaskConfigurations(string jobScheduleName, ITaskConfiguration owningTaskConfiguration, XmlNodeList xmlNodeList)
        {
            List<ITaskConfiguration> taskConfigurations = new List<ITaskConfiguration>();
            try
            {
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    if (xmlNode.Name.Equals(BuildSchedule.Task))
                    {
                        taskConfigurations.Add(buildTaskConfiguration(jobScheduleName, owningTaskConfiguration, xmlNode));
                    }
                }
                return taskConfigurations;
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TaskConfigurationException(GetType().Name, "buildTaskConfigurations", "Unknown task",
                                                     string.Format("build task configurations failed for job {0}",
                                                                   jobScheduleName), e);
            }
        }

        private ITaskConfiguration buildTaskConfiguration(string jobScheduleName, ITaskConfiguration owningTaskConfiguration, XmlNode xmlNode)
        {
            ITaskConfiguration taskConfiguration = createTaskConfiguration(jobScheduleName, null);
            try
            {
                foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                {
                    if (xmlAttribute.Name.Equals(BuildSchedule.TaskName))
                    {
                        if (FactoryHelper.taskNameIsUnconfigured(xmlAttribute.Value))
                        {
                            throw new TaskConfigurationException(GetType().Name, "buildTaskConfiguration", "Unknown", "Task Name is not configured");
                        }
                        else
                        {
                            taskConfiguration.TaskName = xmlAttribute.Value;
                        }
                    }
                    if (xmlAttribute.Name.Equals(BuildSchedule.TaskClassName))
                    {
                        taskConfiguration.TaskClassName = xmlAttribute.Value;
                    }
                }

                foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                {
                    if (childXmlNode.Name.Equals(BuildSchedule.Variables))
                    {
                        taskConfiguration.VariableConfigurations =
                            MainFactory.VariableFactory.buildVariableConfigurations(jobScheduleName, taskConfiguration, childXmlNode.ChildNodes);
                    }
                    if (childXmlNode.Name.Equals(BuildSchedule.SubTasks))
                    {
                        taskConfiguration.ChildTaskConfigurations =
                            buildTaskConfigurations(jobScheduleName, taskConfiguration, childXmlNode.ChildNodes);
                    }
                }
                return createFullTaskConfiguration(taskConfiguration, owningTaskConfiguration);
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TaskConfigurationException(GetType().Name, "buildTaskConfiguration", "Unknown task",
                                                     string.Format("build task configurations failed for job {0}",
                                                                   jobScheduleName), e);
            }
        }

        private ITaskConfiguration createFullTaskConfiguration(ITaskConfiguration loadedTaskConfiguration, ITaskConfiguration owningTaskConfiguration)
        {
            if (!taskClassNameExists(loadedTaskConfiguration.TaskClassName))
            {
                throw new TaskConfigurationException(GetType().Name, "createFullTaskConfiguration", loadedTaskConfiguration.TaskName, string.Format("Default task configuration for class {0} does not exist", loadedTaskConfiguration.TaskClassName));
            }

            ITaskConfiguration defaultTaskConfiguration = getDefaultTaskConfiguration(loadedTaskConfiguration.TaskClassName);
            defaultTaskConfiguration.setOwningTaskConfiguration(owningTaskConfiguration);
            defaultTaskConfiguration.TaskName = loadedTaskConfiguration.TaskName;

            // Append children
            foreach (ITaskConfiguration loadedChildTaskConfiguration in loadedTaskConfiguration.ChildTaskConfigurations)
            {
                defaultTaskConfiguration.ChildTaskConfigurations.Add(createFullTaskConfiguration(loadedChildTaskConfiguration, defaultTaskConfiguration));
            }

            // Append variables & set variables if they exists
            foreach (KeyValuePair<string, IVariableConfiguration> pair in loadedTaskConfiguration.VariableConfigurations)
            {
                pair.Value.setOwningTaskConfiguration(defaultTaskConfiguration);
                if (defaultTaskConfiguration.VariableConfigurations.ContainsKey(pair.Key))
                {
                    defaultTaskConfiguration.VariableConfigurations[pair.Key] = pair.Value;
                }
                else
                {
                    // Append variable
                    defaultTaskConfiguration.VariableConfigurations.Add(pair.Key, pair.Value);
                }
            }

            return defaultTaskConfiguration;
        }

        #endregion

        #region Task Container

        /* Throws FactoryException */

        private ITask createTaskFromName(IJobRunner jobRunner, ITask parentTask, ITaskConfiguration taskConfiguration)
        {
            return createTaskFromName(jobRunner, parentTask, taskConfiguration.TaskName, taskConfiguration.TaskClassName);
        }

        private ITask createTaskFromName(IJobRunner jobRunner, ITask parentTask, string taskName, string taskClassName)
        {
            if (!taskClassNameExists(taskClassName))
            {
                throw new FactoryException(GetType().Name, "createTaskFromName",
                                           string.Format("Task class name {0} does not exist", taskClassName));
            }

            ITask task = null;
            LoadedTask tempTask = null;
            if (_taskList.TryGetValue(taskClassName, out tempTask))
            {
                task = tempTask.getInstance();
            }
            else
            {
                throw new FactoryException(GetType().Name, "CreateTaskFromName",
                                           string.Format("Task class name {0} not existing", taskClassName));
            }
            task.configure(parentTask, taskName, MainFactory.VariableFactory, this, jobRunner);
            return task;
        }

        private class LoadedTask
        {
            private Assembly _assembly;
            private string _taskFullName;

            internal LoadedTask(string taskFullName, Assembly assembly)
            {
                _taskFullName = taskFullName;
                _assembly = assembly;
            }

            public ITask getInstance()
            {
                Object temp = _assembly.CreateInstance(_taskFullName);
                //Object temp = _assembly.CreateInstance(_taskFullName, false,
                //                                       BindingFlags.CreateInstance | BindingFlags.Instance, null,
                //                                       new object[] { jobRunner }, null, null);
                if (temp != null)
                {
                    if (temp is ITask)
                    {
                        return temp as ITask;
                    }
                    else
                    {
                        throw new FactoryException(GetType().Name, "LoadedTask.getInstance",
                                                   String.Format("Task {0} not implementing ITask interface",
                                                                 _taskFullName));
                    }
                }
                else
                {
                    throw new FactoryException(GetType().Name, "LoadedTask.getInstance",
                                               String.Format("Task {0} could not be created", _taskFullName));
                }
            }
        }

        #endregion
    }
}