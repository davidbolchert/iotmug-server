using IoTMug.Device.Models;
using IoTMug.Shared.Helpers;
using Newtonsoft.Json;
using System.IO;

namespace IoTMug.Device.Tools
{
    public class SettingsManager
    {
        public AppSettings AppSettings { get; private set; }

        public SettingsManager(string appSettingRelativePath)
        {
            var path = LocationHelpers.GetLocationFromAssembly(appSettingRelativePath);
            AppSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path));
        }
    }
}
