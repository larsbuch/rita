using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Enumerations;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using Quartz;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobTriggerFactory : IJobTriggerFactory
    {
        public IMainFactory MainFactory { get; set; }

        public List<ITrigger> getJobTriggers(List<ITriggerConfiguration> triggerConfigurations)
        {
            List<ITrigger> triggers = new List<ITrigger>();

            int counter = 1;
            foreach (ITriggerConfiguration triggerConfiguration in triggerConfigurations)
            {
                DateTimeOffset startTimeUTC;
                if (triggerConfiguration.StartUTCDate != null && triggerConfiguration.StartUTCDate.HasValue &&
                    triggerConfiguration.StartUTCDate.Value.CompareTo(DateTime.UtcNow) > 0)
                {
                    startTimeUTC = triggerConfiguration.StartUTCDate.Value;
                }
                else
                {
                    // Start it right away
                    startTimeUTC = DateBuilder.EvenMinuteDate(DateTime.UtcNow);
                }
                // Create Triggers
                ITrigger trigger = null;
                switch (triggerConfiguration.TriggerType)
                {
                    case TriggerType.CronTrigger:
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithCronSchedule(triggerConfiguration.Value, x => x.WithMisfireHandlingInstructionFireAndProceed())
                            .StartAt(startTimeUTC)
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    case TriggerType.StartupTrigger:
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithSchedule(CronScheduleBuilder
                                .CronSchedule("@reboot")
                                .WithMisfireHandlingInstructionFireAndProceed())
                            .StartAt(startTimeUTC)
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    case TriggerType.MinutelyTrigger:
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithSimpleSchedule(x => x
                                .WithIntervalInMinutes(1)
                                .WithMisfireHandlingInstructionFireNow())
                            .StartAt(startTimeUTC)
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    case TriggerType.HourlyTrigger:
                        int minutes = 0;
                        if (!int.TryParse(triggerConfiguration.Value, out minutes))
                        {
                            throw new FactoryException(GetType().Name, "getJobTriggers",
                                                       string.Format("Could not parse minute value {0}",
                                                                     triggerConfiguration.Value));
                        }
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithSchedule(CronScheduleBuilder
                                .CronSchedule(string.Format("{0} * * * *", minutes))
                                .WithMisfireHandlingInstructionFireAndProceed())
                            .StartAt(DateBuilder.EvenHourDate(startTimeUTC).AddMinutes(minutes))
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    case TriggerType.DailyTrigger:
                        string[] values = triggerConfiguration.Value.Split(Misc.TriggerValueSplitter.ToCharArray());
                        int hour;
                        int minute;
                        if (!int.TryParse(values[0], out hour) || !int.TryParse(values[1], out minute))
                        {
                            throw new FactoryException(GetType().Name, "getJobTriggers",
                                                       "Could not parse values in " +
                                                       triggerConfiguration.TriggerType.ToString());
                        }
                        hour = correctToNonUTCTime(hour); // TODO Verify UTC
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithSchedule(CronScheduleBuilder
                                .DailyAtHourAndMinute(hour, minute)
                                .WithMisfireHandlingInstructionFireAndProceed())
                            .StartAt(startTimeUTC)
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    case TriggerType.WeeklyTrigger:
                        string[] weeklyValues = triggerConfiguration.Value.Split(Misc.TriggerValueSplitter.ToCharArray());
                        string weekday = weeklyValues[0];
                        int weekhour;
                        int weekminute;
                        if (!int.TryParse(weeklyValues[1], out weekhour) || !int.TryParse(weeklyValues[2], out weekminute))
                        {
                            throw new FactoryException(GetType().Name, "getJobTriggers",
                                "Could not parse values in " + triggerConfiguration.TriggerType.ToString());
                        }
                        weekhour = correctToNonUTCTime(weekhour); // TODO Verify UTC
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(makeTriggerName(triggerConfiguration.JobSchedule.getJobName(), counter))
                            .WithSchedule(CronScheduleBuilder
                                .WeeklyOnDayAndHourAndMinute(getWeekday(weekday), weekhour, weekminute)
                                .WithMisfireHandlingInstructionFireAndProceed())
                            .StartAt(startTimeUTC)
                            .EndAt(triggerConfiguration.EndUTCDate)
                            .Build();
                        triggers.Add(trigger);
                        break;
                    default:
                        throw new FactoryException(GetType().Name, "getJobTriggers",
                            string.Format("Triggertype {0} is not supported",triggerConfiguration.TriggerType.ToString()));
                }
                counter += 1;
            }
            return triggers;
        }

        private DayOfWeek getWeekday(string weekday)
        {
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            switch (weekday)
            {
                case "Monday":
                    dayOfWeek = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    dayOfWeek = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    dayOfWeek = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    dayOfWeek = DayOfWeek.Saturday;
                    break;
                case "Sunday":
                    dayOfWeek = DayOfWeek.Sunday;
                    break;
            }
            return dayOfWeek;
        }

        private int correctToNonUTCTime(int hour)
        {
            int utcDateTime = DateTime.UtcNow.Hour;
            int normalDateTime = DateTime.Now.Hour;
            hour = hour + normalDateTime - utcDateTime;
            if (hour > 23)
            {
                hour -= 24;
            }
            else if (hour < 0)
            {
                hour += 24;
            }
            return hour;
        }

        public ITrigger getRetryTrigger(string jobName, int retryInterval)
        {
            // Create trigger name and group
            string triggerName = makeTriggerName(jobName, 0);
            string triggerGroup = System.Guid.NewGuid().ToString();

            // Create Trigger for running once
            ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName, triggerGroup).WithSimpleSchedule(x => x.WithIntervalInMinutes(retryInterval)).Build();
            return trigger;
        }

        public List<ITriggerConfiguration> buildTriggerConfigurationList(IJobSchedule jobSchedule, XmlNode xmlNode)
        {
            List<ITriggerConfiguration> triggerConfigurations = new List<ITriggerConfiguration>();
            foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
            {
                if (childXmlNode.Name.Equals(BuildSchedule.Trigger))
                {
                    triggerConfigurations.Add(buildTriggerConfiguration(jobSchedule, childXmlNode));
                }
            }
            return triggerConfigurations;
        }

        private ITriggerConfiguration buildTriggerConfiguration(IJobSchedule jobSchedule, XmlNode xmlNode)
        {
            TriggerType triggerType = TriggerType.UnknownTrigger;
            string value = "";
            DateTime? startUTCDate = DateTime.UtcNow;
            DateTime? stopUTCDate = null;
            try
            {
                foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                {
                    if (xmlAttribute.Name.Equals(BuildSchedule.TriggerType))
                    {
                        triggerType = buildTriggerTypeFromName(jobSchedule,xmlAttribute.Value);
                    }
                    else if (xmlAttribute.Name.Equals(BuildSchedule.Value))
                    {
                        value = xmlAttribute.Value;
                    }
                    else if (xmlAttribute.Name.Equals(BuildSchedule.StartUTCDate))
                    {
                        if (!xmlAttribute.Value.Equals(string.Empty))
                        {
                                startUTCDate = FactoryHelper.getDateTime(xmlAttribute.Value,true);
                        }
                    }
                    else if (xmlAttribute.Name.Equals(BuildSchedule.StopUTCDate))
                    {
                        if (!xmlAttribute.Value.Equals(string.Empty))
                        {
                                stopUTCDate = FactoryHelper.getDateTime(xmlAttribute.Value,true);
                        }
                    }
                }

                return new TriggerConfiguration(jobSchedule, triggerType, value, startUTCDate, stopUTCDate);
            }
            catch (Exception e)
            {
                throw new FactoryException(GetType().Name, "buildTriggerConfiguration",
                                                            string.Format(
                                                                "Could not create a new TriggerConfiguration of type {0} with value {1}",
                                                                triggerType.ToString(), value), e);
            }
        }

        private TriggerType buildTriggerTypeFromName(IJobSchedule jobSchedule, string triggerTypeName)
        {
            if (triggerTypeName.Equals(TriggerType.CronTrigger.ToString()))
            {
                return TriggerType.CronTrigger;
            }
            else if (triggerTypeName.Equals(TriggerType.WeeklyTrigger.ToString()))
            {
                return TriggerType.WeeklyTrigger;
            }
            else if (triggerTypeName.Equals(TriggerType.DailyTrigger.ToString()))
            {
                return TriggerType.DailyTrigger;
            }
            else if (triggerTypeName.Equals(TriggerType.HourlyTrigger.ToString()))
            {
                return TriggerType.HourlyTrigger;
            }
            else if (triggerTypeName.Equals(TriggerType.MinutelyTrigger.ToString()))
            {
                return TriggerType.MinutelyTrigger;
            }
            else if (triggerTypeName.Equals(TriggerType.StartupTrigger.ToString()))
            {
                return TriggerType.StartupTrigger;
            }
            else
            {
                throw new FactoryException(GetType().Name, "buildTriggerTypeFromName",
                                                            string.Format("Tigger type {0} is not supported",
                                                                          triggerTypeName));
            }
        }

        private string makeTriggerName(string jobName, int counter)
        {
            string returnString = "Trigger_" + jobName;
            string numberString;
            if (counter == 0)
            {
                numberString = "";
            }
            else
            {
                numberString = "_" + counter.ToString();
            }

            return returnString + numberString;
        }
    }
}