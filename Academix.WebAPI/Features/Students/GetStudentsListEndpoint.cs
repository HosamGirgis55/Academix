using Academix.Application.Features.Students.Queries.GetStudentsList;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class GetStudentsListEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/students", HandleAsync)
                .WithName("GetStudentsList")
                .WithTags("Students")
                .Produces<ResponseHelper>(200)
                .WithSummary("Get students list")
                .WithDescription("Retrieves a paginated list of students with optional filtering");
        }

        private static async Task<IResult> HandleAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? orderBy = null,
            [FromServices] ISender mediator = null!,
            HttpContext httpContext = null!,
            CancellationToken cancellationToken = default)
        {
            var culture = httpContext.Items["Culture"]?.ToString() ?? "en";
            
            var query = new GetStudentsListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                OrderBy = orderBy
            };

            var result = await mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Error ?? (culture == "ar" ? "فشل في استرداد الطلاب" : "Failed to retrieve students");
                return Results.Ok(new ResponseHelper()
                    .BadRequest(errorMessage, culture));
            }

            var successMessage = culture == "ar" ? "تم استرداد قائمة الطلاب بنجاح" : "Students list retrieved successfully";
            return Results.Ok(new ResponseHelper()
                .Success(result.Value, culture)
                .WithMassage(successMessage));
        }
    }
} 