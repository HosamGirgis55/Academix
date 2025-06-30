using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.RefreshToken;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Authentication;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh-token", RefreshTokenAsync)
            .WithName("RefreshToken")
            .WithTags("Authentication")
            .Accepts<RefreshTokenDto>("application/json")
            .Produces<ResultModel<AuthenticationResult>>(200)
            .Produces<ResultModel<AuthenticationResult>>(400);
    }

    private static async Task<IResult> RefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto,
        IMediator mediator)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenDto.RefreshToken
        };

        var result = await mediator.Send(command) as Result<AuthenticationResult>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 