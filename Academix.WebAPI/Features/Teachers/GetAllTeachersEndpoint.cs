using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class GetAllTeachersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/teachers", HandleAsync)
            .WithName("GetAllTeachers")
            .WithTags("Teachers")
            .Produces<ResponseHelper>(200)
            .Produces<ResponseHelper>(400);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMediator mediator,
        [FromServices] ResponseHelper response,
        [FromServices] ILocalizationService localizationService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid[]? skillIds = null,
        [FromQuery] bool orderByRating = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate pagination parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit page size to prevent abuse

            var query = new GetAllTeachersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SkillIds = skillIds?.ToList() ?? new List<Guid>(),
                OrderByRating = orderByRating
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
