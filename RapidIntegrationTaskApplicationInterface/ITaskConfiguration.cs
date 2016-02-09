using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ITaskConfiguration
    {
        string TaskName { get; set; }
        string TaskClassName { get; set; }
        Dictionary<string,IVariableConfiguration> VariableConfigurations { get; set; }
        List<ITaskConfiguration> ChildTaskConfigurations { get; set; }
        string FullName { get; }
        ITaskConfiguration getCopy(ITaskConfiguration owningTaskConfiguration, bool removePersistedValues);
        void setOwningTaskConfiguration(ITaskConfiguration owningTaskConfiguration);
        void setJobScheduleName(string jobScheduleName);
    }
}
