using Academix.Application.Features.Dashboard.Queries.GetAllSkills;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.DashBoard;

public class GetAllSkillsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/skills", GetAllSkillsAsync)
            .WithName("GetAllSkills")
            .WithTags("Dashboard")
            .Produces(200)
            .Produces(400);
    }

    private static async Task<IResult> GetAllSkillsAsync(IMediator mediator)
    {
        try
        {
            var query = new GetAllSkillsQuery();
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Data = result.Value, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while retrieving skills.", Error = ex.Message });
        }
    }
} 