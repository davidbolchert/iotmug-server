using System;
using System.IO;
using System.Reflection;

namespace IoTMug.Shared.Helpers
{
    public static class LocationHelpers
    {
        public static string GetAssemblyLocation() => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
    }
}
