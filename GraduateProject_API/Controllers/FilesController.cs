using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet("uploads/{*fileName}")]  
        public IActionResult GetFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(path, contentType, Path.GetFileName(fileName));
        }

    }
}
