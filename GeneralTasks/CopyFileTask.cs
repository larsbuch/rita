using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BasicTasks;
using RapidIntegrationTaskApplicationInterface;
using RapidIntegrationTaskApplicationInterface.Enumerations;
using RapidIntegrationTaskApplicationInterface.Exceptions;

namespace GeneralTasks
{
    public class CopyFileTask:AbstractTask
    {
        private static string FileName = "FileName";
        private static string SourceDirectory = "SourceDirectory";
        private static string DestinationDirectory = "DestinationDirectory";
        private static string CreateDestination = "CreateDestination";
        private static string OverwriteIfExist = "OverwriteIfExist";


        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            try
            {
                string fileName = getVariable(FileName).getStringValue();
                string sourceDirectory = getVariable(SourceDirectory).getStringValue();
                string destinationDirectory = getVariable(DestinationDirectory).getStringValue();
                bool createDestination = getVariable(CreateDestination).getBooleanValue();
                bool overwriteIfExists = getVariable(OverwriteIfExist).getBooleanValue();

                if (Directory.Exists(sourceDirectory) && File.Exists(sourceDirectory + DirectoryEnd + fileName))
                {
                    if (!Directory.Exists(destinationDirectory) && createDestination)
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    if (Directory.Exists(destinationDirectory))
                    {
                        if (File.Exists(destinationDirectory + DirectoryEnd + fileName))
                        {
                            if (overwriteIfExists)
                            {
                                File.Copy(sourceDirectory + DirectoryEnd + fileName,
                                          destinationDirectory + DirectoryEnd + fileName);
                            }
                            else
                            {
                                throw new TaskExecutionException(TaskName, "executeTaskInternally",
                                                                 string.Format(
                                                                     "File {0} exists in {1} and overwrite is not allowed",
                                                                     fileName, destinationDirectory));
                            }
                        }
                        else
                        {
                            File.Copy(sourceDirectory + DirectoryEnd + fileName,
                                      destinationDirectory + DirectoryEnd + fileName);
                        }
                    }
                    else
                    {
                        throw new TaskExecutionException(TaskName, "executeTaskInternally",string.Format("Destination directory {0} does not exist",destinationDirectory));
                    }
                }
                else
                {
                    throw new TaskExecutionException(TaskName, "executeTaskInternally",
                                                     string.Format("File {0} does not exist in {1}", fileName,
                                                                   sourceDirectory));
                }
            }
            catch (TaskConfigurationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TaskExecutionException(TaskName, "executeTaskInternally", "Unknown error occured", e);
            }
        }

        protected override void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do Nothing
        }

        public override void resetTask()
        {
            // Do Nothing
        }

        public override void initializeTask()
        {
            createNewEmptyVariable(FileName, VariableType.String, false, "File name to be copied");
            createNewEmptyVariable(SourceDirectory, VariableType.String, false, "Source directory for the file");
            createNewEmptyVariable(DestinationDirectory, VariableType.String, false, "Destination directory for the file");
            createNewEmptyVariable(CreateDestination, VariableType.Boolean, false, "Should the destination directory be created ?");
            createNewEmptyVariable(OverwriteIfExist, VariableType.Boolean, false, "Should the file be overwritten in the destination directory if existing ?");
            base.initializeTask();
        }
    }
}
