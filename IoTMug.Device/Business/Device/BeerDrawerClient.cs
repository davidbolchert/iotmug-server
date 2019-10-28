using IoTMug.Device.Models;
using IoTMug.Core.IoTMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace IoTMug.Device.Business.Device
{
    public class BeerDrawerClient : BaseIoTHubClient
    {
        private Random Random = new Random();
        private BeerDrawerTwinConfiguration _configuration;

        private double _drawerVolume = 100;

        public override void TwinConfigurationHasChanged()
        {
            var jDesired = JObject.Parse(_serverTwin.Properties.Desired.ToJson());
            _configuration = JsonConvert.DeserializeObject<BeerDrawerTwinConfiguration>(jDesired.GetValue("Configuration").ToString());
        }

        public override Task Run(CancellationToken token)
        {
            Task.Run(() => UpdateTwinJob());
            Task.Run(() => SendDrawerVolumeJob());

            while (!token.IsCancellationRequested)
            {
                Task.Run(() => ServeBeer());
                Task.Delay(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
            }

            Program.Logger.Warn("[Client] Cancelation has been requested");
            return Task.CompletedTask;
        }

        private async Task UpdateTwinJob()
        {
            while (true)
            {
                await GetTwinConfigurationAsync();
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        private async Task ServeBeer()
        {
            if (_drawerVolume == 0) return;

            var timestamp = DateTimeOffset.Now;
            var commandType = Random.Next() % 3;

            string value;
            double volume;

            switch (commandType)
            {
                case 1:
                    value = "happy";
                    volume = 0.5;
                    break;
                case 2:
                    value = "pint";
                    volume = 0.5;
                    break;
                case 3:
                default:
                    value = "half";
                    volume = 0.25;
                    break;
            }

            var @event = new Event()
            {
                Name = nameof(ServeBeer),
                Timestamp = timestamp,
                Value = value
            };

            var measure = new Measure()
            {
                Name = nameof(ServeBeer),
                Timestamp = timestamp,
                Unit = "l",
                Value = volume
            };

            _drawerVolume -= volume;

            await SendMessage(@event);
            await SendMessage(measure);
        }

        private async Task SendDrawerVolumeJob()
        {
            while (true)
            {
                var measure = new Measure()
                {
                    Name = "Volume",
                    Timestamp = DateTimeOffset.Now,
                    Unit = "l",
                    Value = _drawerVolume
                };

                await SendMessage(measure);
                if (_drawerVolume == 0) _drawerVolume = 100; 
                await Task.Delay(TimeSpan.FromMinutes(2));
            }
        }
    }
}
