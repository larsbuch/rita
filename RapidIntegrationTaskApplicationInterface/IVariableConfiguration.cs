using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IVariableConfiguration
    {
        string VariableName { get; }
        Enumerations.VariableType VariableType { get; set; }
        bool Persisted { get; set; }
        object Value { get; set; }
        string FullName { get; }
        IVariableConfiguration getCopy(ITaskConfiguration owningTaskConfiguration, bool removePersistedValues);
        void setOwningTaskConfiguration(ITaskConfiguration owningTaskConfiguration);
        bool isUnconfigured();
        string Comment { get; }
    }
}
