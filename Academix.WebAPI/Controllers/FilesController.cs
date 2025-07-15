using Academix.Helper;
using Academix.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class FilesController : ControllerBase
    {
        private readonly FileUploaderHelper _fileUploader;

        public FilesController(FileUploaderHelper fileUploader)
        {
            _fileUploader = fileUploader;
        }

        /// <summary>
        /// Upload a file to cloud storage
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File upload result with URL</returns>
        [HttpPost("upload")]
        [EnableCors("AllowAll")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ResponseHelper), 200)]
        [ProducesResponseType(typeof(ResponseHelper), 400)]
        public async Task<IActionResult> UploadFile(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var culture = HttpContext.Items["Culture"]?.ToString() ?? "en";

            if (file == null || file.Length == 0)
            {
                var noFileMessage = culture == "ar" ? "لم يتم اختيار ملف" : "No file selected.";
                return Ok(new ResponseHelper()
                    .BadRequest(noFileMessage, culture));
            }

            try
            {
                using var stream = file.OpenReadStream();
                var resultUrl = await _fileUploader.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType
                );

                var successMessage = culture == "ar" ? "تم رفع الملف بنجاح" : "File uploaded successfully";
                return Ok(new ResponseHelper()
                    .Success(new { fileUrl = resultUrl }, culture)
                    .WithMassage(successMessage));
            }
            catch (Exception ex)
            {
                var errorMessage = culture == "ar" ? $"فشل رفع الملف: {ex.Message}" : $"File upload failed: {ex.Message}";
                return Ok(new ResponseHelper()
                    .ServerError(errorMessage, culture));
            }
        }

       
       
    }
} 