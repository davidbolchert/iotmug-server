using Newtonsoft.Json.Serialization;

namespace IoTMug.Api.Providers
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || char.IsLower(propertyName, 0))
                return propertyName;

            return char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
        }
    }
}
