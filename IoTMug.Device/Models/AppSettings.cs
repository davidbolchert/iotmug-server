using Microsoft.Azure.Devices.Client;

namespace IoTMug.Device.Models
{
    public struct DeviceTypes
    {
        public const string BEER_DRAWER = "BeerDrawer";
        public const string LIGHT = "Light";
    }

    public class AppSettings
    {
        public string DeviceType { get; set; }
        public string PfxCertificatePath { get; set; }
        public string PfxPassword { get; set; }

        public string IoTHubEndPoint { get; set; }

        private ITransportSettings[] _transportSettings;
        public ITransportSettings[] TransportSettings => _transportSettings ?? CreateTransportSettings();
        private ITransportSettings[] CreateTransportSettings()
        {
            var amqpTransportSettings = new AmqpTransportSettings(TransportType.Amqp_WebSocket_Only);
            _transportSettings = new ITransportSettings[] { amqpTransportSettings };
            return _transportSettings;
        }
    }
}
