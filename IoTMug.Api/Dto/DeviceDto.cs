using IoTMug.Core;
using IoTMug.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTMug.Api.Dto
{
    public class DeviceDto
    {
        public Guid DeviceId { get; set; } = new Guid();

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }

        public bool IsConnected { get; set; } = false;

        [JsonFormat]
        public string Twin { get; set; }

        public Guid DeviceTypeId { get; set; }
        public virtual DeviceType Type { get; set; }
    }
}
