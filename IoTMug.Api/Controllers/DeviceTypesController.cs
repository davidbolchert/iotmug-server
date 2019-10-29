using System;
using System.Linq;
using System.Threading.Tasks;
using IoTMug.Api.Dto;
using IoTMug.Core;
using IoTMug.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IoTMug.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DeviceTypesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        public DeviceTypesController(IDatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
        public IActionResult Get()
        {
            var deviceType = _databaseService.Get<DeviceType>(includeProperties: dt => dt.Include(d => d.Devices)).ToList();
            return Ok(deviceType);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            var deviceType = _databaseService.Get<DeviceType>(dt => dt.DeviceTypeId == id).FirstOrDefault();
            if (deviceType == default(DeviceType)) return NotFound();

            return Ok(deviceType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeviceType deviceType)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessageDto("Invalid Entity Model"));

            var alreadyCreated = _databaseService.Get<DeviceType>(d => d.Name == deviceType.Name).Any();
            if (alreadyCreated) return BadRequest(new HttpMessageDto("A Type with this name already exists. The name must be unique"));

            await _databaseService.AddAsync(deviceType);
            return Created(new Uri($"{Request.Path}/{deviceType.DeviceTypeId}", UriKind.Relative), deviceType);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] DeviceType deviceType)
        {
            if (!ModelState.IsValid) return BadRequest(new HttpMessageDto("Invalid Entity Model"));

            var entity = _databaseService.GetFirstOrDefault<DeviceType>(d => d.DeviceTypeId == deviceType.DeviceTypeId);

            if (entity == null) return NotFound();

            if (entity.Name != deviceType.Name) return BadRequest(new HttpMessageDto("Name cannot be changed once the type has been created"));

            entity.DefaultTwin = deviceType.DefaultTwin;

            await _databaseService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deviceType = _databaseService.Get<DeviceType>(dt => dt.DeviceTypeId == id).FirstOrDefault();
            if (deviceType == default(DeviceType)) return NotFound();
            
            if (deviceType.Devices.Count > 0) return BadRequest(new HttpMessageDto("There are one or several devices linked to this type."));

            await _databaseService.DeleteAsync(deviceType);
            return NoContent();
        }
    }
}
