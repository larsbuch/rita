using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Enumerations
{
    public enum TriggerType
    {
        CronTrigger,
        StartupTrigger,
        MinutelyTrigger,
        HourlyTrigger,
        DailyTrigger,
        UnknownTrigger,
        WeeklyTrigger
    }
}
