using IoTMug.Core.IoTMessages;
using IoTMug.Shared.Helpers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMug.Device.Business.Device
{
    public abstract class BaseIoTHubClient : IIoTHubClient
    {
        public const string CONFIGURATION_NODE = "Configuration";

        public const uint OperationTimeoutInSeconds = 5;
        private readonly DeviceAuthenticationWithX509Certificate _deviceAuthentication;

        public DeviceClient Connector { get; private set; }
        public bool IsConnected { get; set; }
        protected Twin _serverTwin { get; set; }

        public BaseIoTHubClient()
        {
            _deviceAuthentication = InitializeDeviceAuthentication();
        }

        private DeviceAuthenticationWithX509Certificate InitializeDeviceAuthentication()
        {
            var certificatePath = LocationHelpers.GetLocationFromAssembly(Program.SettingManager.AppSettings.PfxCertificatePath);
            if (!File.Exists(certificatePath)) throw new FileNotFoundException("Authentication certificate not found");

            var certificate = new X509Certificate2(certificatePath, Program.SettingManager.AppSettings.PfxPassword);
            return new DeviceAuthenticationWithX509Certificate(certificate.GetNameInfo(X509NameType.SimpleName, false), certificate);
        }

        public async Task ConnectAsync()
        {
            try
            {
                Connector = DeviceClient.Create(Program.SettingManager.AppSettings.IoTHubEndPoint, null, _deviceAuthentication, Program.SettingManager.AppSettings.TransportSettings);
                Connector.OperationTimeoutInMilliseconds = OperationTimeoutInSeconds * 1000;
                // Set retry policy
                var retryPolicy = new ExponentialBackoff(int.MaxValue, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(100), TimeSpan.FromMilliseconds(1000));
                Connector.SetRetryPolicy(retryPolicy);

                // Set Methods
                await Connector.SetMethodHandlerAsync(IoTHubMethods.UPDATE_TWIN, OnUpdateTwin, null);

                // Open connection
                await Connector.OpenAsync(new CancellationTokenSource(TimeSpan.FromSeconds(OperationTimeoutInSeconds)).Token);
                Program.Logger.Info($"[IoTHub] Device {_deviceAuthentication.DeviceId} is connected");
                await GetTwinConfigurationAsync();
                IsConnected = true;

            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, $"Connection error to the IoTHub");
                IsConnected = false;
            }
        }

        public async Task DisconnectAsync()
        {
            Program.Logger.Info($"[IotHub] Device {_deviceAuthentication.DeviceId} connection close");
            try
            {
                await Connector.CloseAsync(new CancellationTokenSource(TimeSpan.FromSeconds(OperationTimeoutInSeconds)).Token);
                Connector.Dispose();
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, $"[Device] Something went wrong during the closing the connection.");
                throw;
            }
        }

        public async Task GetTwinConfigurationAsync()
        {
            try
            {
                _serverTwin = await Connector.GetTwinAsync();
                TwinConfigurationHasChanged();
                Program.Logger.Info($"[IotHub] Twin has been updated");
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, $"[IoTHub] Exception thrown while trying to get twin from iot hub");
                return;
            }
        }

        public abstract void TwinConfigurationHasChanged();

        private async Task<MethodResponse> OnUpdateTwin(MethodRequest methodRequest, object userContext)
        {
            await GetTwinConfigurationAsync();
            return new MethodResponse((int) HttpStatusCode.OK);
        }

        protected async Task SendMessage(Measure measure)
        {
            var data = Encoding.Default.GetBytes(JsonConvert.SerializeObject(measure));
            await SendMessage(data);
            Program.Logger.Info($"[Device] Send Measure {measure.Name} | value : {measure.Value}");
        }

        protected async Task SendMessage(Event @event)
        {
            var data = Encoding.Default.GetBytes(JsonConvert.SerializeObject(@event));
            await SendMessage(data);
            Program.Logger.Info($"[Device] Send Event {@event.Name} | value : {@event.Value}");
        }

        private async Task SendMessage(byte[] data)
        {
            // Avoid connection issues
            try
            {
                var sendingTask = Connector.SendEventAsync(new Message(data));
                var result = Task.WaitAny(new[] { sendingTask }, TimeSpan.FromSeconds(OperationTimeoutInSeconds));
                if (result == -1) throw new TimeoutException();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, "[Device] Exception thrown while trying to send data");
            }
        }

        public abstract Task Run(CancellationToken token);
    }
}
