using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicTasks;
using ChronographedTaskInterface;

namespace GeneralTasks
{
    public class IncludingDll:AbstractTask  
    {
        private WebDAVClient.WebDAVClient _webDavClient;

        public IncludingDll()
        {
            _webDavClient = new WebDAVClient.WebDAVClient();
        }

        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do nothing
        }

        protected override void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do nothing
        }

        public override void resetTask()
        {
            // Do nothing
        }
    }
}
