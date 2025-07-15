using Academix.Application.Features.Dashboard.Queries.GetSkillById;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.DashBoard;

public class GetSkillByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/skills/{id:guid}", GetSkillByIdAsync)
            .WithName("GetSkillById")
            .WithTags("Dashboard")
            .Produces(200)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> GetSkillByIdAsync(Guid id, IMediator mediator)
    {
        try
        {
            var query = new GetSkillByIdQuery { Id = id };
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Data = result.Value, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while retrieving the skill.", Error = ex.Message });
        }
    }
} 