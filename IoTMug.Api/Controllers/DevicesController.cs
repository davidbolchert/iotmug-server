using System;
using System.Linq;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IoTMug.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        public DevicesController(IDatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
       // [Authorize]
        public IActionResult Get()
        {
            var devices = _databaseService.Get<Device>(includeProperties: d => d.Include(dt => dt.Type)).ToList();
            return Ok(devices);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public IActionResult Get([FromRoute] Guid deviceId)
        {
            var device = _databaseService.Get<Device>((d => d.DeviceId == deviceId), includeProperties: d => d.Include(dt => dt.Type)).FirstOrDefault();
            if (device == default(Device)) return NotFound();

            return Ok(device);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Device device)
        {
            if (!ModelState.IsValid) return BadRequest(new { error = "Invalid Entity Model" });

            var alreadyCreated = _databaseService.Get<Device>(d => d.Name == device.Name).Any();
            if (alreadyCreated) return BadRequest(new { error = "A Device with this name already exists. The name must be Unique" });

            _databaseService.Add(device);
            return Created(new Uri($"{Request.Path}/{device.DeviceId}", UriKind.Relative), device);
        }
    }
}
