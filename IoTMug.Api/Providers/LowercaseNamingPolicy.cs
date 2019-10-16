using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace IoTMug.Api.Providers
{
    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name, 0)) return name;

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
