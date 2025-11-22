using DataProcessor.Business;
using DataProcessor.Business.Contracts;
using DataProcessor.Business.Dtos;
using DataProcessor.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataProcessor.API.Controllers
{
    [Route("api/persons")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonProcessorService _processor;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonProcessorService processor, ILogger<PersonController> logger)
        {
            _processor = processor;
            _logger = logger;
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
                _logger.LogInformation("CSV upload finished");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during CSV upload");
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
