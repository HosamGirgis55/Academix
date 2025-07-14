using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Dashboard.Query.Teacher.GetTeachers;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Domain.Enums;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard
{
    public class GetTeachersEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/GetTeachers", HandleAsync)
                .WithName("GetTeachers")
                .WithTags("Dashboard")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromQuery] Status status,
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            CancellationToken cancellationToken,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = new GetTeacherQuery 
                { 
                    Status = status,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await mediator.Send(query, cancellationToken);

                if (result.IsSuccess)
                    return Results.Ok(response.Success(result.Value));

                return Results.BadRequest(response.BadRequest(result.Error));
            }
            catch (Exception)
            {
                var message = localizationService.GetLocalizedString("TeacherGetAllFailed");
                return Results.BadRequest(response.BadRequest(message));
            }
        }
    }
}
