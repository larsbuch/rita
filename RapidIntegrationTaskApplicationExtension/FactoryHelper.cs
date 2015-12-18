using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerExtension
{
    public static class FactoryHelper
    {
        public static XmlDocument loadXmlDocument(string directoryName, string fileName, bool validate)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string xmlDocumentPlacement = directoryName + Misc.DirectoryEnd + fileName + Misc.XmlFileEnding;
            XmlReader reader = null;
            try
            {
                if (!File.Exists(xmlDocumentPlacement))
                {
                    throw new RukisIntegrationTaskhandlerException(string.Format("The file {0} does not exists",xmlDocumentPlacement));
                }
                if (validate)
                {
                    // Set the validation settings.
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                    //settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                    settings.ValidationEventHandler += new ValidationEventHandler(validateOfSchemaFailedEventHandler);

                    // Create the XmlReader object.
                    reader = XmlReader.Create(xmlDocumentPlacement, settings);
                }
                else
                {
                    reader = XmlReader.Create(xmlDocumentPlacement);
                }
                // Possible single call to read
                xmlDocument.Load(reader);
                return xmlDocument;
            }
                catch(RukisIntegrationTaskhandlerException)
                {
                    throw;
                }
            catch (Exception e)
            {
                throw new RukisIntegrationTaskhandlerException(String.Format("Could not read file {1} in directory {0}", directoryName, fileName), e);
            }
        }


        private static void validateOfSchemaFailedEventHandler(object sender, ValidationEventArgs args)
        {
            throw new FactoryException("JobScheduleFactory", "getXmlDocumentAndValidateIt",
                                                        "Validation of " + Misc.JobScheduleFileName + " failed");
        }

        public static void saveXmlDocument(XmlDocument xmlDocument, string directoryName, string fileName)
        {
            try
            {
                xmlDocument.Save(directoryName + Misc.DirectoryEnd + fileName + Misc.XmlFileEnding);
            }
            catch (Exception e)
            {
                throw new RukisIntegrationTaskhandlerException(String.Format("Could not write file {1} in directory {0}", directoryName, fileName), e);
            }
        }


        public static bool variableIsUnconfigured(IVariableConfiguration variableConfiguration)
        {
            if (variableConfiguration.Value != null && variableConfiguration.Value is string && (variableConfiguration.Value as string).Equals(Task.UnconfiguredVariable))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DateTime? getDateTime(string dateTimeString, bool throwException)
        {
            DateTime tempDateTime;
            if (!DateTime.TryParseExact(dateTimeString, Misc.DateTimeFormat, CultureInfo.InvariantCulture,
                                   DateTimeStyles.AssumeUniversal, out tempDateTime) && throwException)
            {
                throw new RukisIntegrationTaskhandlerException(string.Format("Cannot parse {0} in the format {1}",dateTimeString,Misc.DateTimeFormat));
            }
            return tempDateTime;
        }

        public static string getDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(Misc.DateTimeFormat);
        }

        public static bool variableExists(List<IVariable> variables, string variableName)
        {
            bool found = false;
            foreach (IVariable variable in variables)
            {
                if(variable.VariableName.Equals(variableName))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool taskNameIsUnconfigured(string taskName)
        {
            if (taskName.Equals(Task.UnconfiguredTaskName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool variableIsUnconfigured(string variableValue)
        {
            if (variableValue.Equals(Task.UnconfiguredVariable))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
