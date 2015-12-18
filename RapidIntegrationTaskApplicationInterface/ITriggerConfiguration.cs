using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface.Enumerations;

namespace RukisIntegrationTaskhandlerInterface
{
    public interface ITriggerConfiguration
    {
        DateTime? StartUTCDate { get; }
        DateTime? EndUTCDate { get; }
        string Value { get; }
        TriggerType TriggerType { get; }
        IJobSchedule JobSchedule { get; }
    }
}
