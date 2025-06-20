using Academix.Helper;
using Academix.Helpers;
using Academix.WebAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Files
{
    public class UploadFileEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/files/upload", HandleAsync)
                .WithName("UploadFile")
                .WithTags("Files")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .DisableAntiforgery()
                .WithSummary("Upload a file")
                .WithDescription("Uploads a file to cloud storage and returns the file URL");
        }

        private static async Task<IResult> HandleAsync(
            IFormFile file,
            [FromServices] FileUploaderHelper fileUploader,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            
            if (file == null || file.Length == 0)
            {
                var noFileMessage = culture == "ar" ? "لم يتم اختيار ملف" : "No file selected.";
                return Results.Ok(new ResponseHelper()
                    .BadRequest(noFileMessage, culture));
            }

            try
            {
                using var stream = file.OpenReadStream();
                var resultUrl = await fileUploader.UploadFileAsync(
                    stream, 
                    file.FileName, 
                    file.ContentType
                );

                var successMessage = culture == "ar" ? "تم رفع الملف بنجاح" : "File uploaded successfully";
                return Results.Ok(new ResponseHelper()
                    .Success(new { fileUrl = resultUrl }, culture)
                    .WithMassage(successMessage));
            }
            catch (Exception ex)
            {
                var errorMessage = culture == "ar" ? $"فشل رفع الملف: {ex.Message}" : $"File upload failed: {ex.Message}";
                return Results.Ok(new ResponseHelper()
                    .ServerError(errorMessage, culture));
            }
        }
    }
} 