using System;
using System.Linq;
using System.Threading.Tasks;
using IoTMug.Api.Dto;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using IoTMug.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using IoTMug.Shared.Extensions;
using System.Security.Cryptography.X509Certificates;
using IoTMug.Api.Dto.Adapters;

namespace IoTMug.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICertificateService _certificateService;
        private readonly IProvisioningService _provisionningService;
        private readonly IIoTHubService _ioTHubService;
        private readonly IDtoAdapter<Device, DeviceDto> _deviceDtoAdapter;

        public DevicesController(IDatabaseService databaseService, 
            ICertificateService certificateService, 
            IProvisioningService provisionningService,
            IIoTHubService ioTHubService)
        {
            _databaseService = databaseService;
            _certificateService = certificateService;
            _provisionningService = provisionningService;
            _ioTHubService = ioTHubService;
            _deviceDtoAdapter = new DeviceDtoAdapter(_ioTHubService);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var devices = _databaseService.Get<Device>(includeProperties: d => d.Include(dt => dt.Type)).ToList();
            var devicesDto = _deviceDtoAdapter.ConvertToDtos(devices);
            return Ok(devicesDto);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            var device = _databaseService.Get<Device>((d => d.DeviceId == id), includeProperties: d => d.Include(dt => dt.Type)).FirstOrDefault();
            if (device == default(Device)) return NotFound();

            var deviceDto = _deviceDtoAdapter.ConvertToDto(device);
            return Ok(deviceDto);
        }

        [HttpGet]
        [Route("certify/{id}")]
        public async Task<IActionResult> Certify([FromRoute] Guid id)
        {
            var device = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == id);
            if (device == default(Device)) return NotFound();

            if (device.PfxCertificate == null || !device.IsRegistered)
            {
                var certificate = _certificateService.GenerateDeviceCertificate(device.CommonName);
                var password = device.GetPassword();

                device.PfxCertificate = certificate.Export(X509ContentType.Pfx, password);

                var registrationCertificate = device.GetRegistrationCertificate(); // Work Around to avoid registrastion issues
                try
                {
                    device.IsRegistered = await _provisionningService.RegisterAsync(registrationCertificate);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
               
                await _databaseService.UpdateAsync(device);
                await _ioTHubService.UpdateDeviceTwin(device.CommonName, device.TwinData);
            }

            return File(device.PfxCertificate, "application/x-pkcs12", $"{device.Name}.pfx");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeviceDto deviceDto)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessageDto("Invalid Entity Model"));

            var alreadyCreated = _databaseService.Get<Device>(d => d.Name == deviceDto.Name).Any();
            if (alreadyCreated) return BadRequest(new HttpMessageDto("A Device with this name already exists. The name must be Unique"));

            var device = _deviceDtoAdapter.ConvertToEntity(deviceDto);
            await _databaseService.AddAsync(device);

            return Created(new Uri($"{Request.Path}/{device.DeviceId}", UriKind.Relative), _deviceDtoAdapter.ConvertToDto(device));
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] DeviceDto deviceDto)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessageDto("Invalid Entity Model"));

            var entity = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == deviceDto.DeviceId);
            if (entity.Name != deviceDto.Name) return BadRequest(new HttpMessageDto("Name cannot be changed once the device has been created"));

            entity.TwinData = JObject.Parse(deviceDto.Twin);
            entity.DeviceTypeId = deviceDto.DeviceTypeId;

            await _databaseService.UpdateAsync(entity);

            if (entity.IsRegistered) await _ioTHubService.UpdateDeviceTwin(entity.CommonName, entity.TwinData);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var device = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == id);
            if (device == default(Device)) return NotFound();

            await _databaseService.DeleteAsync(device);
            return NoContent();
        }
    }
}
