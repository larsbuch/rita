using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Enumerations;

namespace RapidIntegrationTaskApplicationExtension
{
    public class TriggerConfiguration:ITriggerConfiguration
    {

        public TriggerConfiguration(IJobSchedule jobSchedule, TriggerType triggerType, string value, DateTimeOffset? startUTCDate, DateTimeOffset? endUTCDate)
        {
            JobSchedule = jobSchedule;
            TriggerType = triggerType;
            Value = value;
            StartUTCDate = startUTCDate;
            EndUTCDate = endUTCDate;
        }
        
        public DateTimeOffset? StartUTCDate{ get; protected set;}

        public DateTimeOffset? EndUTCDate{ get; protected set;}

        public string Value{ get; protected set;}

        public TriggerType TriggerType{ get; protected set;}

        public IJobSchedule JobSchedule{ get; protected set;}
    }
}
