using IoTMug.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTMug.Core
{
    public class Device
    {
        public Guid DeviceId { get; set; } = new Guid();

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] PfxCertificate { get; set; }

        public bool IsRegistered { get; set; } = false;

        [JsonFormat]
        public string Twin { get; set; }

        [ForeignKey("DeviceTypeId")]
        public Guid DeviceTypeId { get; set; }
        public virtual DeviceType Type { get; set; }
    }
}
