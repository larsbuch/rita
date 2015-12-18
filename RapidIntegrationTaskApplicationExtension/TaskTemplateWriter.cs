using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Xml;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;

namespace RukisIntegrationTaskhandlerExtension
{
    public class TaskTemplateWriter : ITaskTemplateWriter
    {
        public void writeTaskTemplates(Dictionary<string, ITaskConfiguration> taskConfigurationDictionary)
        {
            try
            {
                string fullDirectoryName = GlobalVariables.getServiceLocation() + Misc.TaskTemplateFolderPlacement;

                // Check Existance
                if (!Directory.Exists(fullDirectoryName))
                {
                    if (!Directory.CreateDirectory(fullDirectoryName).Exists)
                    {
                        throw new RukisIntegrationTaskhandlerException(String.Format("Task Template location {0} does not exist",
                                                                           fullDirectoryName));
                    }
                }
                // Delete Existing Templates
                String[] files = Directory.GetFiles(fullDirectoryName);
                foreach (string fileName in files)
                {
                    File.Delete(fileName);
                }

                // Create Templates
                foreach (var pair in taskConfigurationDictionary)
                {
                    string taskConfiguration = Misc.XmlEncoding + getTaskXml(pair.Value);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(taskConfiguration);
                    FactoryHelper.saveXmlDocument(xmlDocument, fullDirectoryName, pair.Key);
                }
            }
            catch (RukisIntegrationTaskhandlerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RukisIntegrationTaskhandlerException(String.Format("Could not write task templates to {0}", Misc.TaskTemplateFolderPlacement), e);
            }
        }

        private string getTaskXml(ITaskConfiguration taskConfiguration)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<task taskname=\"" + Task.UnconfiguredTaskName + "\" taskclassname=\"" + taskConfiguration.TaskClassName + "\">");
            stringBuilder.AppendLine("    <variables>");
            if(taskConfiguration.VariableConfigurations.Count > 0)
            {
                stringBuilder.Append(getVariableXml(taskConfiguration.VariableConfigurations));
            }
            stringBuilder.AppendLine("    </variables>");
            stringBuilder.AppendLine("    <subtasks>");
            if (taskConfiguration.ChildTaskConfigurations.Count > 0)
            {
                foreach (ITaskConfiguration subTaskConfiguration in taskConfiguration.ChildTaskConfigurations)
                {
                    stringBuilder.Append(getTaskXml(subTaskConfiguration));                    
                }
            }
            stringBuilder.AppendLine("    </subtasks>");
            stringBuilder.AppendLine("</task>");
            return stringBuilder.ToString();
        }

        private string getVariableXml(Dictionary<string,IVariableConfiguration> variableConfigurations)
        {
            StringBuilder stringBuilder = new StringBuilder();
                foreach(IVariableConfiguration variableConfiguration in variableConfigurations.Values)
                {
                    stringBuilder.AppendLine("        <variable variablename=\"" + variableConfiguration.VariableName + "\" variabletype=\"" + variableConfiguration.VariableType.ToString() + "\" persisted=\"" + variableConfiguration.Persisted + "\">");
                    stringBuilder.AppendLine("            <variablevalue value=\"" + Task.UnconfiguredVariable + "\" />");
                    if(variableConfiguration.VariableType == VariableType.DateTime)
                    {
                        stringBuilder.AppendLine("            <!-- Format is yyyy-mm-ddThh:mm fx 2009-03-14T15:45 -->");
                    }
                    if(!variableConfiguration.Comment.Equals(string.Empty))
                    {
                        stringBuilder.AppendLine("            <!-- " + variableConfiguration.Comment + " -->");
                    }
                    stringBuilder.AppendLine("        </variable>");
                }
            return stringBuilder.ToString();
        }
    }
}
