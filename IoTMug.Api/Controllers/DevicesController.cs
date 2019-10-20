using System;
using System.Linq;
using System.Threading.Tasks;
using IoTMug.Api.Dto;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IoTMug.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        public DevicesController(IDatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
        public IActionResult Get()
        {
            var devices = _databaseService.Get<Device>(includeProperties: d => d.Include(dt => dt.Type)).ToList();
            return Ok(devices);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get([FromRoute] Guid deviceId)
        {
            var device = _databaseService.Get<Device>((d => d.DeviceId == deviceId), includeProperties: d => d.Include(dt => dt.Type)).FirstOrDefault();
            if (device == default(Device)) return NotFound();

            return Ok(device);
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
            entity.Type = device.Type;

            await _databaseService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid deviceId)
        {
            var device = _databaseService.GetFirstOrDefault<Device>(d => d.DeviceId == deviceId);
            if (device == default(Device)) return NotFound();

            await _databaseService.DeleteAsync(device);
            return NoContent();
        }
    }
}
