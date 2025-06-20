using Academix.Application.Features.Students.Commands.DeleteStudent;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class DeleteStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/students/{id:guid}", HandleAsync)
                .WithName("DeleteStudent")
                .WithTags("Students")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(404)
                .WithSummary("Delete a student")
                .WithDescription("Removes a student from the system");
        }

        private static async Task<IResult> HandleAsync(
            [FromRoute] Guid id,
            [FromServices] ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            var command = new DeleteStudentCommand(id);
            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Error ?? (culture == "ar" ? "الطالب غير موجود" : "Student not found");
                return Results.Ok(new ResponseHelper()
                    .NotFound(errorMessage, culture));
            }

            var successMessage = culture == "ar" ? "تم حذف الطالب بنجاح" : "Student deleted successfully";
            return Results.Ok(new ResponseHelper()
                .Success(null, culture)
                .WithMassage(successMessage));
        }
    }
} 