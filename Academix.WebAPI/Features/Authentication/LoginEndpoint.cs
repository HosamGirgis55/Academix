using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.Login;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Authentication;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("Login")
            .WithTags("Authentication")
            .Accepts<LoginDto>("application/json")
            .Produces<ResultModel<AuthenticationResult>>(200)
            .Produces<ResultModel<AuthenticationResult>>(400);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginDto loginDto,
        IMediator mediator)
    {
        var command = new LoginCommand
        {
            Email = loginDto.Email,
            Password = loginDto.Password
        };

        var result = await mediator.Send(command) as Result<AuthenticationResult>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 