using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Constants
{
    public static class Context
    {
        public static readonly string TaskConfiguration = "TaskConfiguration";
        public static readonly string ErrorTaskConfiguration = "ErrorTaskConfiguration";
        public static readonly string CurrentRetry = "CurrentRetry";
        public static readonly string MaxRetry = "MaxRetry";
        public static readonly string RetryInterval = "RetryInterval";
        public static readonly string CurrentTaskName = "CurrentTaskName";
        public static readonly string LifetimeName = "LifetimeName";
    }
}
