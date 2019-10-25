using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IoTMug.Api.Dto;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
        public DevicesController(IDatabaseService databaseService, 
            ICertificateService certificateService, 
            IProvisioningService provisionningService,
            IIoTHubService ioTHubService)
        {
            _databaseService = databaseService;
            _certificateService = certificateService;
            _provisionningService = provisionningService;
            _ioTHubService = ioTHubService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var devices = _databaseService.Get<Device>(includeProperties: d => d.Include(dt => dt.Type)).ToList();
            return Ok(devices);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            var device = _databaseService.Get<Device>((d => d.DeviceId == id), includeProperties: d => d.Include(dt => dt.Type)).FirstOrDefault();
            if (device == default(Device)) return NotFound();

            return Ok(device);
        }

        [HttpGet]
        [Route("certify/{id}")]
        public async Task<IActionResult> Certify([FromRoute] Guid id)
        {
            var device = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == id);
            if (device == default(Device)) return NotFound();

            if (device.PfxCertificate == null || !device.IsRegistered)
            {
                var certificate = _certificateService.GenerateDeviceCertificate(device.Name);

                var certificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Assets/1664.pfx");
                //certificate = new X509Certificate2(certificatePath, "1234");

                device.IsRegistered = await _provisionningService.RegisterAsync(certificate);
                device.PfxCertificate = certificate.Export(X509ContentType.Pfx, "1234");
                // await _databaseService.UpdateAsync(device);
                // await UpdateTwin(device);
            }

            return File(device.PfxCertificate, "application/x-pkcs12", $"{device.Name}.pfx");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Device device)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessage("Invalid Entity Model"));

            var alreadyCreated = _databaseService.Get<Device>(d => d.Name == device.Name).Any();
            if (alreadyCreated) return BadRequest(new HttpMessage("A Device with this name already exists. The name must be Unique"));

            await _databaseService.AddAsync(device);
            return Created(new Uri($"{Request.Path}/{device.DeviceId}", UriKind.Relative), device);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] Device device)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessage("Invalid Entity Model"));

            var entity = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == device.DeviceId);
            if (entity.Name != device.Name) return BadRequest(new HttpMessage("Name cannot be changed once the device has been created"));

            entity.Twin = device.Twin;
            entity.DeviceTypeId = device.DeviceTypeId;

            await _databaseService.UpdateAsync(entity);

            if (entity.IsRegistered) await UpdateTwin(entity);

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

        private async Task UpdateTwin(Device device)
        {
            if (device.DeviceId != Guid.Empty)
            {
                await _ioTHubService.UpdateDeviceTwin(device.DeviceId.ToString(), JObject.Parse(device.Twin));
                await _ioTHubService.ExecuteMethodOnDevice(IoTHubMethods.UPDATE_TWIN, device.DeviceId.ToString());
            }
        }
    }
}
