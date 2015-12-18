using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface ITaskTemplateWriter
    {
        void writeTaskTemplates(Dictionary<string, ITaskConfiguration> taskConfigurationDictionary);
    }
}
