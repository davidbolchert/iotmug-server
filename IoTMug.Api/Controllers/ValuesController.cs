using System.Collections.Generic;
using IoTMug.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IoTMug.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public ValuesController(ICertificateService certificateService) => _certificateService = certificateService;

        // GET api/v1/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            System.IO.File.WriteAllBytes("test.pfx", _certificateService.GenerateDeviceCertificate("test", "test"));
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
