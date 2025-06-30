using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.ChangePassword;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Authentication;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/change-password", ChangePasswordAsync)
            .WithName("ChangePassword")
            .WithTags("Authentication")
            .RequireAuthorization()
            .Accepts<ChangePasswordDto>("application/json")
            .Produces<ResultModel<bool>>(200)
            .Produces<ResultModel<bool>>(400);
    }

    private static async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordDto changePasswordDto,
        HttpContext httpContext,
        IMediator mediator)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var command = new ChangePasswordCommand
        {
            UserId = userId,
            CurrentPassword = changePasswordDto.CurrentPassword,
            NewPassword = changePasswordDto.NewPassword
        };

        var result = await mediator.Send(command) as Result<bool>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 