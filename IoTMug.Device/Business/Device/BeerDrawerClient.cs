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
            _configuration = JsonConvert.DeserializeObject<BeerDrawerTwinConfiguration>(jDesired.GetValue(CONFIGURATION_NODE).ToString());
            Program.Logger.Info(JsonConvert.SerializeObject(_configuration, Formatting.Indented));
        }

        public override async Task Run(CancellationToken token)
        {
            await Task.CompletedTask;

#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            Task.Run(() => SendDrawerVolumeJob(token));

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Task.Run(() => ServeBeer());
                    Task.Delay(TimeSpan.FromSeconds(45)).GetAwaiter().GetResult();
                }
                Program.Logger.Warn("[Client] Cancelation has been requested");
            });

#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }

        private async Task ServeBeer()
        {
            if (_drawerVolume == 0) return;

            var timestamp = DateTimeOffset.Now;
            var commandType = Random.Next() % 3;
            var loss = Random.Next() % 3 * 0.01;

            string value;
            double volume;

            switch (commandType)
            {
                case 1:
                    value = "happy";
                    volume = 0.5 + loss;
                    break;
                case 2:
                    value = "pint";
                    volume = 0.5 + loss;
                    break;
                case 3:
                default:
                    value = "half";
                    volume = 0.25 + loss;
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

        private async Task SendDrawerVolumeJob(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var measure = new Measure()
                {
                    Name = "Volume",
                    Timestamp = DateTimeOffset.Now,
                    Unit = "l",
                    Value = _drawerVolume
                };

                await SendMessage(measure);
                if (_drawerVolume < 0.5) _drawerVolume = 100; 
                await Task.Delay(TimeSpan.FromMinutes(2));
            }
        }
    }
}
