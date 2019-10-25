using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IoTMug.Services.Implementations
{
    public class IoTHubServiceImplementation : IIoTHubService
    {
        private readonly IoTHubSettings _settings;

        public IoTHubServiceImplementation(IOptions<IoTHubSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task UpdateDeviceTwin(string deviceId, JObject jsonTwinDesired)
        {
            var properties = new TwinProperties();

            using var registryManager = RegistryManager.CreateFromConnectionString(_settings.ConnectionString);
            var twin = await registryManager.GetTwinAsync(deviceId);

            var configurationNode = new JObject { "Configuration", jsonTwinDesired };
            twin.Properties.Desired = new TwinCollection(configurationNode.ToString());

            using var hasher = SHA1.Create();
            var etag = Encoding.UTF8.GetString(hasher.ComputeHash(Encoding.UTF8.GetBytes(jsonTwinDesired.ToString())));

            await registryManager.UpdateTwinAsync(deviceId, twin, etag);

            var device = await registryManager.GetDeviceAsync(deviceId);
            if (device.ConnectionState == DeviceConnectionState.Connected) await this.ExecuteMethodOnDevice(IoTHubMethods.UPDATE_TWIN, deviceId);
        }

        public async Task<int> ExecuteMethodOnDevice(string methodName, string deviceId)
        {
            using var iotHubService = ServiceClient.CreateFromConnectionString(_settings.ConnectionString);
            var methodRequest = new CloudToDeviceMethod(methodName, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            try
            {
                var methodResponse = await iotHubService.InvokeDeviceMethodAsync(deviceId, methodRequest);
                return methodResponse.Status;
            }
            catch (Exception)
            {
                // log exception here and handle it if you want
                throw;
            }
        }

        public async Task<bool> IsDeviceConnected(string deviceId)
        {
            using var registryManager = RegistryManager.CreateFromConnectionString(_settings.ConnectionString);
            var device = await registryManager.GetDeviceAsync(deviceId);
            return device != null && device.ConnectionState == DeviceConnectionState.Connected;
        }
    }
}
