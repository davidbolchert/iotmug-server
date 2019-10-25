using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using System;
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
            using var security = new SecurityProviderX509Certificate(certificate);
            using var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.WebSocketOnly);
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
            catch (Exception)
            {
                // Handle exception here if you want
                throw;
            }
        }
    }
}
