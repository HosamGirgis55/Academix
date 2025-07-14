using Academix.Application.Features.Dashboard.Commands.DeleteSkill;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.DashBoard;

public class DeleteSkillEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/dashboard/skills/{id:guid}", DeleteSkillAsync)
            .WithName("DeleteSkill")
            .WithTags("Dashboard")
            .Produces(200)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> DeleteSkillAsync(Guid id, IMediator mediator)
    {
        try
        {
            var command = new DeleteSkillCommand { Id = id };
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while deleting the skill.", Error = ex.Message });
        }
    }
} 