using IoTMug.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTMug.Core
{
    public class DeviceType
    {
        [Key]
        public Guid DeviceTypeId { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        [JsonFormat]
        public string DefaultTwin { get; set; }

        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
