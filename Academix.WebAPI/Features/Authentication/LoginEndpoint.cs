using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.Login;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Linq;
using System.Text.Json;
using System.Text;

namespace Academix.WebAPI.Features.Authentication;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("Login")
            .WithTags("Authentication")
            .Produces<ResultModel<AuthenticationResult>>(200)
            .Produces<ResultModel<AuthenticationResult>>(400);
    }

    private static async Task<IResult> LoginAsync(
        IMediator mediator, [FromBody] LoginDto loginDto)
    {
        try
        {
            var command = new LoginCommand
            {
                Email = loginDto.Email,
                Password = loginDto.Password,
                FcmToken = loginDto.FcmToken
            };

            var result = await mediator.Send(command);
            if (result == null)
            {
                var nullResultError = new ResultModel<AuthenticationResult>
                {
                    Success = false,
                    Message = "An unexpected error occurred during authentication.",
                    Errors = new List<string> { "Command result was null" }
                };
                return Results.BadRequest(nullResultError);
            }
            
            var resultModel = result.ToResultModel();

            return result.IsSuccess
                ? Results.Ok(resultModel)
                : Results.BadRequest(resultModel);
        }
        catch (Exception ex)
        {
            var errorResult = new ResultModel<AuthenticationResult>
            {
                Success = false,
                Message = "An unexpected error occurred while processing the request.",
                Errors = new List<string> { ex.Message }
            };
            return Results.BadRequest(errorResult);
        }
    }
} 