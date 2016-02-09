using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Variables;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface IVariablePersister
    {
        void registerPersistedVariable(IVariable variable);
        bool checkForPersistedValue(string variableFullName);
        void gettingPersistedVariable(IVariableConfiguration copyVariableConfiguration);
        void settingPersistedVariable(IVariable variable);
    }
}
