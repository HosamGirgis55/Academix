using Academix.Application.Features.Dashboard.Commands.UpdateSkill;
using Academix.Domain.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard;

public class UpdateSkillEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dashboard/skills/{id:guid}", UpdateSkillAsync)
            .WithName("UpdateSkill")
            .WithTags("Dashboard")
            .Produces(200)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> UpdateSkillAsync(
        Guid id, 
        [FromBody] UpdateSkillDto dto, 
        IMediator mediator)
    {
        try
        {
            var command = new UpdateSkillCommand
            {
                Id = id,
                NameAr = dto.NameAr,
                NameEn = dto.NameEn
            };

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while updating the skill.", Error = ex.Message });
        }
    }
} 