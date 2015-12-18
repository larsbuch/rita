using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Enumerations;

namespace RukisIntegrationTaskhandlerExtension
{
    public class TriggerConfiguration:ITriggerConfiguration
    {

        public TriggerConfiguration(IJobSchedule jobSchedule, TriggerType triggerType, string value, DateTime? startUTCDate, DateTime? endUTCDate)
        {
            JobSchedule = jobSchedule;
            TriggerType = triggerType;
            Value = value;
            StartUTCDate = startUTCDate;
            EndUTCDate = endUTCDate;
        }
        
        public DateTime? StartUTCDate{ get; protected set;}

        public DateTime? EndUTCDate{ get; protected set;}

        public string Value{ get; protected set;}

        public TriggerType TriggerType{ get; protected set;}

        public IJobSchedule JobSchedule{ get; protected set;}
    }
}
