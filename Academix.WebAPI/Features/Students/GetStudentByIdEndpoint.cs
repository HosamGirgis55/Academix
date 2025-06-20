using Academix.Application.Features.Students.Queries.GetStudentById;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class GetStudentByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/students/{id:guid}", HandleAsync)
                .WithName("GetStudentById")
                .WithTags("Students")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(404)
                .WithSummary("Get student by ID")
                .WithDescription("Retrieves a specific student's information by their unique identifier");
        }

        private static async Task<IResult> HandleAsync(
            [FromRoute] Guid id,
            [FromServices] ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            var query = new GetStudentByIdQuery(id);
            var result = await mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Error ?? (culture == "ar" ? "الطالب غير موجود" : "Student not found");
                return Results.Ok(new ResponseHelper()
                    .NotFound(errorMessage, culture));
            }

            var successMessage = culture == "ar" ? "تم استرداد الطالب بنجاح" : "Student retrieved successfully";
            return Results.Ok(new ResponseHelper()
                .Success(result.Value, culture)
                .WithMassage(successMessage));
        }
    }
} 