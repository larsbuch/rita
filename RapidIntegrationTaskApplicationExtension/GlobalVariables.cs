using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RukisIntegrationTaskhandlerExtension
{
    public static class GlobalVariables
    {
        // Returns the service assembly location
        public static string getServiceLocation()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string name = assembly.GetName().Name;
            string locationWithName = assembly.Location;
            if (locationWithName == null)
            {
                locationWithName = "";
            }
            string locationWithoutName = locationWithName.Replace(string.Format("\\{0}.exe", name), "");
            return locationWithoutName;
        }
    }
}