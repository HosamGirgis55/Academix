using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.ResetPassword;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Authentication;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/reset-password", ResetPasswordAsync)
            .WithName("ResetPassword")
            .WithTags("Authentication")
            .Accepts<ResetPasswordDto>("application/json")
            .Produces<ResultModel<bool>>(200)
            .Produces<ResultModel<bool>>(400);
    }

    private static async Task<IResult> ResetPasswordAsync(
        [FromBody] ResetPasswordDto resetPasswordDto,
        IMediator mediator)
    {
        var command = new ResetPasswordCommand
        {
            Email = resetPasswordDto.Email,
            Token = resetPasswordDto.Token,
            NewPassword = resetPasswordDto.NewPassword
        };

        var result = await mediator.Send(command) as Result<bool>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 