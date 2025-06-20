using Academix.Application.Features.Students.Commands.UpdateStudent;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class UpdateStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/students/{id:guid}", HandleAsync)
                .WithName("UpdateStudent")
                .WithTags("Students")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .Produces<ResponseHelper>(404)
                .WithSummary("Update student information")
                .WithDescription("Updates an existing student's information");
        }

        private static async Task<IResult> HandleAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateStudentRequest request,
            [FromServices] ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            
            var command = new UpdateStudentCommand
            {
                Id = id,
                FirstName = request.FirstName,
                FirstNameAr = request.FirstNameAr,
                LastName = request.LastName,
                LastNameAr = request.LastNameAr,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };

            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Contains("not found") ?? false)
                {
                    var notFoundMessage = culture == "ar" ? "الطالب غير موجود" : "Student not found";
                    return Results.Ok(new ResponseHelper()
                        .NotFound(notFoundMessage, culture));
                }

                var errorMessage = result.Error ?? (culture == "ar" ? "فشل في تحديث الطالب" : "Failed to update student");
                return Results.Ok(new ResponseHelper()
                    .BadRequest(errorMessage, culture));
            }

            var successMessage = culture == "ar" ? "تم تحديث الطالب بنجاح" : "Student updated successfully";
            return Results.Ok(new ResponseHelper()
                .Success(null, culture)
                .WithMassage(successMessage));
        }
    }

    public record UpdateStudentRequest(
        string FirstName,
        string FirstNameAr,
        string LastName,
        string LastNameAr,
        string Email,
        DateTime DateOfBirth
    );
} 