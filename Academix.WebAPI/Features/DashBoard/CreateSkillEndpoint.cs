using Academix.Application.Features.Dashboard.Commands.AddSkills;
using Academix.Domain.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard;

public class CreateSkillEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dashboard/skills", CreateSkillAsync)
            .WithName("CreateSkill")
            .WithTags("Dashboard")
            .Produces(200)
            .Produces(400);
    }

    private static async Task<IResult> CreateSkillAsync(
        IMediator mediator, 
        [FromBody] CreateSkillDto dto)
    {
        try
        {
            var command = new AddSkillsCommand
            {
                NameEn = dto.NameEn
            };

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while creating the skill.", Error = ex.Message });
        }
    }
} 