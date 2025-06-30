using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.ForgotPassword;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Authentication;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/forgot-password", ForgotPasswordAsync)
            .WithName("ForgotPassword")
            .WithTags("Authentication")
            .Accepts<ForgotPasswordDto>("application/json")
            .Produces<ResultModel<string>>(200);
    }

    private static async Task<IResult> ForgotPasswordAsync(
        [FromBody] ForgotPasswordDto forgotPasswordDto,
        IMediator mediator)
    {
        var command = new ForgotPasswordCommand
        {
            Email = forgotPasswordDto.Email
        };

        var result = await mediator.Send(command) as Result<string>;
        var resultModel = result!.ToResultModel();

        return Results.Ok(resultModel);
    }
} 