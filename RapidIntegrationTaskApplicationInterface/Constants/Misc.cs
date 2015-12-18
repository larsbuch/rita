using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Constants
{
    public static class Misc
    {
        // File Placement
#if DEBUG
        public static readonly string JobScheduleFolderPlacement = @"";
        public static readonly string TaskFolderPlacement = @"";
        public static readonly string TaskTemplateFolderPlacement = @"\TaskTemplates";
#else
        public static readonly string JobScheduleFolderPlacement = @"\JobSchedule";		  
        public static readonly string TaskFolderPlacement = @"\Tasks";		  
        public static readonly string TaskTemplateFolderPlacement = @"\TaskTemplates";
#endif

        
        public static readonly string TriggerValueSplitter = ":";
        public static readonly string FolderDivider = @"\";
        public static readonly string JobScheduleFileName = @"JobSchedules";
        public static readonly string XmlEncoding = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
        public static readonly string DirectoryEnd = "\\";
        public static readonly string XmlFileEnding = ".xml";
        public static readonly string TaskFilter = "*.dll";
        public static readonly string Task = "Task";
        public static readonly string DateTimeFormat = "yyyy\\-MM\\-dd\\THH:mm";
        public static readonly string PersistedVariablesFile = "PersistedVariables.bin";
        public static readonly int PersistenceUnusedMax = 50;
        public static readonly char MailRecipientsSplitter = ';';
    }
}
