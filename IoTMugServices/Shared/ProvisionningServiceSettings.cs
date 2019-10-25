
namespace IoTMug.Services.Shared
{
    public class ProvisionningServiceSettings
    {
        public string DeviceProvisionningServiceEndPoint { get; set; }
        public string DeviceProvisionningServiceScopeId { get; set; }

        public string RootCertificatePath { get; set; }
        public string IntermediateCertificatePath { get; set; }
    }
}
