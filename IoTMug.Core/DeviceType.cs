using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTMug.Core
{
    public class DeviceType
    {
        public Guid DeviceTypeId { get; private set; } = new Guid();

        [Required]
        public string Name { get; set; }

        [Required]
        public string DefaultTwinData { get; private set; }
        [NotMapped]
        public JObject DefaultTwin
        {
            get => JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DefaultTwinData) ? new JObject().ToString() : DefaultTwinData);
            set => DefaultTwinData = value.ToString();
        }
    }
}
