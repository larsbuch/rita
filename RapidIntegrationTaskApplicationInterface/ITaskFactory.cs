using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ITaskFactory
    {
        IMainFactory MainFactory { get; set; }
        void loadFactory();
        ITask buildTask(IJobRunner jobRunner,ITaskConfiguration taskConfiguration);
        ITaskConfiguration buildGroupTaskConfiguration(string jobScheduleName, XmlNode xmlNode);
        Dictionary<string, ITaskConfiguration> getTaskConfigurationList();
        ITaskConfiguration getDefaultTaskConfiguration(string taskClassName);
        Dictionary<string, ITaskConfiguration> getDefaultTaskConfigurationList();
        ITask createNewChildTask(string taskName, string taskClassName, ITask parentTask);
        bool taskClassNameExists(string taskClassName);
        ITaskConfiguration createTaskConfiguration(string jobScheduleName,ITaskConfiguration owningTaskConfiguration);
        ITaskConfiguration createTaskConfigurationWithEmptySchedule(string jobScheduleName, ITaskConfiguration owningTaskConfiguration);
    }
}
