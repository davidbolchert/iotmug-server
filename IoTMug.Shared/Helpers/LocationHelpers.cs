using System;
using System.IO;
using System.Reflection;

namespace IoTMug.Shared.Helpers
{
    public static class LocationHelpers
    {
        public static string GetAssemblyLocation() => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static string GetLocationFromAssembly(string path) => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
    }
}
