using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlazorWasmFileUpload.Server.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class UploadController : ControllerBase
	{
		private readonly IWebHostEnvironment env;
		public UploadController(IWebHostEnvironment env) => this.env = env;

		[HttpPost]
		[DisableRequestSizeLimit]
		public async Task<IActionResult> PostFile(IFormFile uploadFile)
		{
			if (uploadFile == null || uploadFile.Length == 0) return BadRequest();

			// Temporary, does NOT work when published, use only for local debugging
			string contentRoot = env.ContentRootPath.Replace("Server", "Client");

			string uploadPath = Path.Combine(contentRoot, "wwwroot", "uploads");
			string fullFilePath = Path.Combine(uploadPath, uploadFile.FileName);

			if (!Directory.Exists(uploadPath))
				Directory.CreateDirectory(uploadPath);

			using (var fs = new FileStream(fullFilePath, FileMode.Append))
				await uploadFile.CopyToAsync(fs);

			return Created(fullFilePath, null);
		}
	}
}
