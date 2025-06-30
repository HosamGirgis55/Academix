using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.UpdateProfile;
using Academix.Application.Features.Authentication.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Authentication;

public class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/auth/profile", UpdateProfileAsync)
            .WithName("UpdateProfile")
            .WithTags("Authentication")
            .RequireAuthorization()
            .Accepts<UpdateProfileDto>("application/json")
            .Produces<ResultModel<UserProfileDto>>(200)
            .Produces<ResultModel<UserProfileDto>>(400);
    }

    private static async Task<IResult> UpdateProfileAsync(
        [FromBody] UpdateProfileDto updateProfileDto,
        HttpContext httpContext,
        IMediator mediator)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var command = new UpdateProfileCommand
        {
            UserId = userId,
            FirstName = updateProfileDto.FirstName,
            LastName = updateProfileDto.LastName,
            ProfilePictureUrl = updateProfileDto.ProfilePictureUrl,
            CountryId = updateProfileDto.CountryId,
            Gender = updateProfileDto.Gender
        };

        var result = await mediator.Send(command) as Result<UserProfileDto>;
        var resultModel = result!.ToResultModel();

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 