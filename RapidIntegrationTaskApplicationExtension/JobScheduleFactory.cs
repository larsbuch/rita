using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Constants;
using RapidIntegrationTaskApplicationInterface.Exceptions;
using System.Xml.Schema;

namespace RapidIntegrationTaskApplicationExtension
{
    public class JobScheduleFactory : IJobScheduleFactory
    {
        public IMainFactory MainFactory { get; set; }

        public List<IJobSchedule> getJobSchedules()
        {
            try
            {
                XmlDocument xmlDocument = getXmlDocumentAndValidateIt();
                return buildScheduleList(xmlDocument);
            }
            catch (Exception e)
            {
                throw new FactoryException(GetType().Name, "getJobSchedules",
                                                            "Failing when trying to read jobschedules", e);
            }
        }


        private List<IJobSchedule> buildScheduleList(XmlDocument xmlDocument)
        {
            XmlNodeList xmlNodeList = xmlDocument.ChildNodes;
            List<IJobSchedule> jobSchedules = new List<IJobSchedule>();
            foreach (XmlNode childXmlNode in xmlNodeList)
            {
                if (childXmlNode.Name == BuildSchedule.JobSchedules)
                {
                    foreach (XmlNode scheduleXmlNode in childXmlNode.ChildNodes)
                    {
                        if (scheduleXmlNode.Name == BuildSchedule.JobSchedule)
                        {
                            try
                            {
                                jobSchedules.Add(buildSchedule(scheduleXmlNode));
                            }
                            catch (TaskConfigurationException e)
                            {
                                string scheduleName = "Unknown Schedule";
                                try
                                {
                                    scheduleName = scheduleXmlNode.Attributes.GetNamedItem(BuildSchedule.JobName).Value;
                                }
                                catch(Exception)
                                {
                                    // Do nothing
                                }
                                MainFactory.SystemLogger.logScheduleLoadFailed(scheduleName,e);
                            }
                        }
                    }
                }
            }
            return jobSchedules;
        }

        private IJobSchedule buildSchedule(XmlNode xmlNode)
        {
            IJobSchedule jobSchedule = createNewJobSchedule();
            ITaskConfiguration taskConfiguration = null;
            ITaskConfiguration errorTaskConfiguration = null;
            string jobName = "Unknown Schedule";
            int retryInterval = 0;
            int maxRetry = 0;
            List<ITriggerConfiguration> triggerConfigurations = null;
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                if (xmlAttribute.Name == BuildSchedule.JobName)
                {
                    jobName = xmlAttribute.Value;
                }
                else if (xmlAttribute.Name == BuildSchedule.RetryInterval)
                {
                    if (!int.TryParse(xmlAttribute.Value, out retryInterval))
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildSchedule","Unknown Task",
                                                                    string.Format("Could not parse RetryInterval in job {0}", jobName));
                    }
                }
                else if (xmlAttribute.Name == BuildSchedule.MaxRetry)
                {
                    if (!int.TryParse(xmlAttribute.Value, out maxRetry))
                    {
                        throw new TaskConfigurationException(GetType().Name, "buildSchedule", "Unknown Task",
                                                                    string.Format("Could not parse MaxRetry in job {0}", jobName));
                    }
                }
            }
            foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
            {
                if (childXmlNode.Name == BuildSchedule.Tasks)
                {
                    taskConfiguration = MainFactory.TaskFactory.buildGroupTaskConfiguration(jobName, childXmlNode);
                }
                else if (childXmlNode.Name == BuildSchedule.ErrorTasks)
                {
                    errorTaskConfiguration = MainFactory.TaskFactory.buildGroupTaskConfiguration(jobName, childXmlNode);
                }
                else if (childXmlNode.Name == BuildSchedule.Triggers)
                {
                    triggerConfigurations =
                        MainFactory.JobTriggerFactory.buildTriggerConfigurationList(jobSchedule, childXmlNode);
                }
            }

            jobSchedule.configure(taskConfiguration, errorTaskConfiguration, jobName, retryInterval, maxRetry,
                                  triggerConfigurations);


            return jobSchedule;
        }

        public IJobSchedule createNewJobSchedule()
        {
            return new JobSchedule();
        }

        private XmlDocument getXmlDocumentAndValidateIt()
        {
            try
            {
                string fullDirectoryName = GlobalVariables.getServiceLocation() + Misc.JobScheduleFolderPlacement;

                return FactoryHelper.loadXmlDocument(fullDirectoryName, Misc.JobScheduleFileName, true);
            }
            catch (Exception e)
            {
                throw new FactoryException(GetType().Name, "getXmlDocumentAndValidateIt",
                                                            "Could not read  " + Misc.JobScheduleFileName,e);
            }
        }
    }
}