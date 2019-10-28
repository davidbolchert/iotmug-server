using IoTMug.Device.Business;
using IoTMug.Device.Business.Device;
using IoTMug.Device.Models;
using IoTMug.Device.Tools;
using System;
using System.Threading;

namespace IoTMug.Device
{
    class Program
    {
        private const string _APP_SETTINGS_DEFAULT_PATH = "Assets/app-settings.json";

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static readonly SettingsManager SettingManager = new SettingsManager(_APP_SETTINGS_DEFAULT_PATH);

        private readonly IIoTHubClient _device;
        static void Main(string[] args) => new Program();

        public Program()
        {
            switch (SettingManager.AppSettings.DeviceType)
            {
                case DeviceTypes.BEER_DRAWER:
                    _device = new BeerDrawerClient();
                    break;

                case DeviceTypes.LIGHT:
                    break;

                default:
                    Logger.Error($"The device type {SettingManager.AppSettings.DeviceType} is not Handle");
                    break;
            }

            if (_device != null)
            {
                _device.ConnectAsync().GetAwaiter().GetResult();
                var cancelationTokenSource = new CancellationTokenSource();
                _device.Run(cancelationTokenSource.Token).GetAwaiter().GetResult();

                Console.ReadKey();
                cancelationTokenSource.Cancel();
            }
        }
    }
}
