using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IoTMug.Services.Implementations
{
    public class ProvisioningServiceImplementation : IProvisioningService
    {
        private readonly ProvisionningServiceSettings _settings;

        public ProvisioningServiceImplementation(IOptions<ProvisionningServiceSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> RegisterAsync(X509Certificate2 certificate)
        {
            //var rootCertificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _settings.RootCertificatePath);
            //var root = new X509Certificate2(rootCertificatePath, "Mug2910.");
            //var intermediateCertificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _settings.IntermediateCertificatePath);
            //var intermediate = new X509Certificate2(intermediateCertificatePath, "Iot2910.");

            using var security = new SecurityProviderX509Certificate(certificate);
            using var transport = new ProvisioningTransportHandlerHttp();
            var provisionningDeviceClient = ProvisioningDeviceClient.Create(
                _settings.DeviceProvisionningServiceEndPoint,
                _settings.DeviceProvisionningServiceScopeId,
                security,
                transport);

            try
            {
                var result = await provisionningDeviceClient.RegisterAsync();
                return result.Status == ProvisioningRegistrationStatusType.Assigned;
            }
            catch (Exception ex)
            {
                var test = ex;
                // Handle exception here if you want
                return false;
                throw;
            }
        }
    }
}
