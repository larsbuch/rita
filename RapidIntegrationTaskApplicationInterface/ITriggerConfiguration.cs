using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Enumerations;

namespace RapidIntegrationTaskApplicationInterface
{
    public interface ITriggerConfiguration
    {
        DateTimeOffset? StartUTCDate { get; }
        DateTimeOffset? EndUTCDate { get; }
        string Value { get; }
        TriggerType TriggerType { get; }
        IJobSchedule JobSchedule { get; }
    }
}
