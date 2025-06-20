using Academix.Application.Features.Students.Commands.CreateStudent;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class CreateStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/students", HandleAsync)
                .WithName("CreateStudent")
                .WithTags("Students")
                .Produces<ResponseHelper>(201)
                .Produces<ResponseHelper>(400)
                .WithSummary("Create a new student")
                .WithDescription("Creates a new student in the system with the provided information");
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] CreateStudentCommand command,
            [FromServices] ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Error ?? (culture == "ar" ? "فشل في إنشاء الطالب" : "Failed to create student");
                return Results.Ok(new ResponseHelper()
                    .BadRequest(errorMessage, culture));
            }

            var successMessage = culture == "ar" ? "تم إنشاء الطالب بنجاح" : "Student created successfully";
            return Results.Created(
                $"/api/students/{result.Value}", 
                new ResponseHelper()
                    .Created(new { studentId = result.Value }, culture)
                    .WithMassage(successMessage)
            );
        }
    }
} 