using IoTMug.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTMug.Core
{
    public class Device
    {
        public Guid DeviceId { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string CommonName => Name.Replace("_", "__").Replace(' ', '_');

        public string Description { get; set; }
        public byte[] PfxCertificate { get; set; }

        public bool IsRegistered { get; set; } = false;

        [JsonFormat]
        public string Twin { get; set; }
        
        [NotMapped]
        public JObject TwinData
        {
            get => JObject.Parse(Twin ?? new JObject().ToString());
            set => Twin = value.ToString();
        }

        [ForeignKey("DeviceTypeId")]
        public Guid DeviceTypeId { get; set; }
        public virtual DeviceType Type { get; set; }
    }
}
