using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmFileUpload.Server.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        public FileUploadController(IWebHostEnvironment env) => this.env = env;

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> PostFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest();

            // Temporary, does NOT work when published, use only for local debugging
            string webRootPath = env.ContentRootPath.Replace("Server", "Client");
            Console.WriteLine($"***** webRootPath: {webRootPath}");

            string uploadPath = Path.Combine(webRootPath, "wwwroot", "uploads");
            string fullFilePath = Path.Combine(uploadPath, file.FileName);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            if (System.IO.File.Exists(fullFilePath))
                System.IO.File.Delete(fullFilePath);

            using (var fs = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return Ok();
        }
    }
}
