using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{

    [Route("upload")]
    public class UploadController(IUnitOfWork unitOfWork, IOptions<ConnectionStrings> appConnection, IWebHostEnvironment environment, IOptions<ApplicationSettings> appSettings) : Controller
    {
        private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        private readonly ConnectionStrings _appConnection = appConnection.Value ?? throw new ArgumentNullException(nameof(appConnection));
        public readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings.Value));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        [HttpPost]
        [Route("uploadFile")]
        [Consumes("multipart/form-data")]
        public async Task<dynamic> UploadFile([FromQuery(Name = "type")] string type, [FromQuery(Name = "fileName")] string fileName)
        {
            try
            {
                string folder = FilesType.Profile;
                string uploadsRootFolder = Path.Combine(_environment.ContentRootPath + $"/uploads", folder);
                var path = Path.Combine(_environment.ContentRootPath, uploadsRootFolder);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                IFormFileCollection files = Request.Form.Files;

                fileName = Utilities.RemoveSpecialCharacters(fileName);
                fileName = Utilities.RemoveDiacritics(fileName);

                string fullFileName = string.Empty;

                foreach (var file in files)
                {

                    if (file == null || file.Length == 0)
                    {
                        continue;
                    }

                    string lastFragment = file.FileName.Split('.').Last();
                    string upperType = char.ToUpperInvariant(type[0]) + type.Substring(1);

                    string extension = System.IO.Path.GetExtension(fileName);
                    string result = fileName.Substring(0, fileName.Length - extension.Length);
                    fullFileName = $"{result}_{DateTime.Now:yyyyMMddHHmmss}{file.FileName.Substring(file.FileName.Length - (lastFragment.Length + 1), lastFragment.Length + 1)}";

                    var filePath = Path.Combine(uploadsRootFolder, fullFileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                }

                string url = Utilities.GetUrl(_appSettings);
                string urlPath = url + Path.Combine(Path.Combine($"uploads/", folder), fullFileName);
                return new { file = fullFileName, urlPath };

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private static string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string lastFragment = fileName.Split('.').Last();
            string extension = System.IO.Path.GetExtension(fileName);
            string result = fileName.Substring(0, fileName.Length - extension.Length);
            strFileName = $"{result}_{DateTime.Now:yyyyMMddHHmmss}{fileName.Substring(fileName.Length - (lastFragment.Length + 1), lastFragment.Length + 1)}";
            return strFileName;
        }

        private static async Task<byte[]> GetByteArrayFromImageAsync(IFormFile file)
        {
            using var target = new MemoryStream();
            await file.CopyToAsync(target);
            return target.ToArray();
        }

    }
}
