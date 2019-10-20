using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTMug.Core
{
    public class Device
    {
        public Guid DeviceId { get; private set; } = new Guid();

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] PfxCertificate { get; set; }

        public string TwinData { get; private set; }
        [NotMapped]
        public JObject Twin
        {
            get => JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(TwinData) ? new JObject().ToString() : TwinData);
            set => TwinData = value.ToString();
        }

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType Type { get; set; }
    }
}
