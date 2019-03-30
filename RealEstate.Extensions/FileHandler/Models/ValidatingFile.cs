using Microsoft.AspNetCore.Http;

namespace RealEstate.Extensions.FileHandler.Models
{
    public class ValidatingFile
    {
        public bool Success { get; set; }
        public IFormFile File { get; set; }
    }
}