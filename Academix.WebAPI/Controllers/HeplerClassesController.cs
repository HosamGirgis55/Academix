using System;
using Academix.Helper;
using Academix.Helpers;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Academix.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeplerClassesController : ControllerBase
    {
        private readonly FileUploaderHelper _fileUploader;
        public HeplerClassesController(FileUploaderHelper helper) {

            _fileUploader = helper;

        }



        [HttpPost("upload")]
        public async Task<IActionResult> uploadFle(IFormFile file)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            using var stream = file.OpenReadStream();
            var resultUrl = await _fileUploader.UploadFileAsync(stream, file.FileName, file.ContentType);
            

            return Ok (new ResponseHelper().Success().WithData(resultUrl));
        }

        
    }
}
