using System.ComponentModel.DataAnnotations;

namespace DataProcessor.Business.Dtos
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
