using Academix.Application.Features.Dashboard.Commands.CreateSpecialization;
using Academix.Domain.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard;

public class CreateSpecializationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dashboard/specializations", CreateSpecializationAsync)
            .WithName("CreateSpecialization")
            .WithTags("Dashboard")
           // .RequireAuthorization()
            .Produces(200)
            .Produces(400);
    }

    private static async Task<IResult> CreateSpecializationAsync(
        IMediator mediator, 
        [FromBody] CreateSpecializationDto dto)
    {
        try
        {
            var command = new CreateSpecializationCommand
            {
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
            return Results.BadRequest(new { Success = false, Message = "An error occurred while creating the specialization.", Error = ex.Message });
        }
    }
} 