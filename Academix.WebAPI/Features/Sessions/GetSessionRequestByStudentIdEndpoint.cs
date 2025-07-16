using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Sessions.Queries.GetAllSessionForTeacher;
using Academix.Application.Features.Sessions.Queries.GetSessionRequestByStudentId;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Domain.Entities;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Sessions
{
    public class GetSessionRequestByStudentIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/GetSessionRequestByStudentId", HandleAsync)
                .WithName("GetSessionRequestByStudentId")
                .WithTags("Sessions")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
                //.RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(
        [FromServices] IMediator mediator,
        [FromServices] ResponseHelper response,
        [FromServices] ILocalizationService localizationService,
        [FromQuery] Guid studentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Limit page size to prevent abuse

                var query = new GetSessionRequestByStudentIdQuery
                {
                    StudentId = studentId,
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
                var message = localizationService.GetLocalizedString("GetAllSissionFailed");
                return Results.BadRequest(response.BadRequest(message));
            }
        }


    }
}
