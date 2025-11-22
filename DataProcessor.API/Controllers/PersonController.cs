using DataProcessor.Business.Contracts;
using DataProcessor.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessor.API.Controllers
{
    [Route("api/persons")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonProcessorService _processor;

        public PersonController(IPersonProcessorService processor)
        {
            _processor = processor;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public IActionResult UploadCsv([FromForm] FileUploadDto model)
        {
            var file = model.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var result = _processor.ProcessCsv(stream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
