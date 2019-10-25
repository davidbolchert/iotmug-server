using System.Collections.Generic;
using System.Linq;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using IoTMug.Shared.Extensions;
using Newtonsoft.Json.Linq;

namespace IoTMug.Api.Dto.Adapters
{
    public class DeviceDtoAdapter : IDtoAdapter<Device, DeviceDto>
    {
        private readonly IIoTHubService _ioTHubService;
        public DeviceDtoAdapter(IIoTHubService ioTHubService) => _ioTHubService = ioTHubService;

        public Device ConvertToEntity(DeviceDto dto)
        {
            var entity = new Device()
            {
                DeviceId = dto.DeviceId,
                Name = dto.Name,
                Description = dto.Description,
                DeviceTypeId = dto.DeviceTypeId,
                TwinData = JObject.Parse(dto.Twin)
            };

            return entity;
        }

        public DeviceDto ConvertToDto(Device entity)
        {
            var dto = new DeviceDto()
            {
                DeviceId = entity.DeviceId,
                Name = entity.Name,
                Password = entity.GetPassword(),
                Description = entity.Description,
                Twin = entity.TwinData.ToString(),
                DeviceTypeId = entity.DeviceTypeId,
                Type = entity.Type,
                IsConnected = _ioTHubService.IsDeviceConnected(entity.CommonName).GetAwaiter().GetResult()
            };

            return dto;
        }

        public IEnumerable<DeviceDto> ConvertToDtos(IEnumerable<Device> entities)
        {
            var dtos = new List<DeviceDto>();
            entities.ToList().ForEach(e => dtos.Add(ConvertToDto(e)));
            return dtos;
        }

        public IEnumerable<Device> ConvertToEntities(IEnumerable<DeviceDto> dtos)
        {
            var entities = new List<Device>();
            dtos.ToList().ForEach(d => entities.Add(ConvertToEntity(d)));
            return entities;
        }
    }
}
