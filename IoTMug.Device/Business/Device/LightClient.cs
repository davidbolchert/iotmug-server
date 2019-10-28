using IoTMug.Device.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMug.Device.Business.Device
{
    public class LightClient : BaseIoTHubClient
    {
        private LightTwinConfiguration _configuration;
        public override void TwinConfigurationHasChanged()
        {
            var jDesired = JObject.Parse(_serverTwin.Properties.Desired.ToJson());
            _configuration = JsonConvert.DeserializeObject<LightTwinConfiguration>(jDesired.GetValue(CONFIGURATION_NODE).ToString());
            Program.Logger.Info($"[Light] The device has been Switch {(_configuration.SwitchOn ? "on" : "off")}");
        }

        public override async Task Run(CancellationToken token)
        {
            await Task.CompletedTask;
        }
    }
}
