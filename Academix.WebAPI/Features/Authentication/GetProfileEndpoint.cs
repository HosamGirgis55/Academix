using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using Academix.Application.Features.Authentication.Queries.GetProfile;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Authentication;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/profile", GetProfileAsync)
            .WithName("GetProfile")
            .WithTags("Authentication")
            .RequireAuthorization()
            .Produces<ResultModel<UserProfileDto>>(200)
            .Produces<ResultModel<UserProfileDto>>(404)
            .Produces(401);
    }

    private static async Task<IResult> GetProfileAsync(
        HttpContext httpContext,
        IMediator mediator)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetProfileQuery
        {
            UserId = userId
        };

        var result = await mediator.Send(query) as Result<UserProfileDto>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.NotFound(resultModel);
    }
} 