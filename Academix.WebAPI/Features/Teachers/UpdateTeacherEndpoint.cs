using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Commands.UpdateTeacher;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Teachers;

public class UpdateTeacherEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/teachers/profile", UpdateTeacherProfileAsync)
            .WithName("UpdateTeacherProfile")
            .WithTags("Teachers")
            .RequireAuthorization()
            .Accepts<UpdateTeacherDto>("application/json")
            .Produces<ResultModel>(200)
            .Produces<ResultModel>(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> UpdateTeacherProfileAsync(
        [FromBody] UpdateTeacherDto updateTeacherDto,
        HttpContext httpContext,
        IMediator mediator)
    {
        // Get user ID from JWT token
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        // Check if user has Teacher role
        var userRoles = httpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value);
        if (!userRoles.Contains("Teacher"))
        {
            return Results.Forbid();
        }

        // Map DTO to Command
        var command = new UpdateTeacherCommand
        {
            UserId = userId,
            FirstName = updateTeacherDto.FirstName,
            LastName = updateTeacherDto.LastName,
            ProfilePictureUrl = updateTeacherDto.ProfilePictureUrl,
            CountryId = updateTeacherDto.CountryId,
            Gender = updateTeacherDto.Gender,
            Bio = updateTeacherDto.Bio,
            Salary = updateTeacherDto.Salary,
            AdditionalInterests = updateTeacherDto.AdditionalInterests,
            Educations = updateTeacherDto.Educations,
            Certificates = updateTeacherDto.Certificates,
            Skills = updateTeacherDto.Skills,
            TeachingAreaIds = updateTeacherDto.TeachingAreaIds,
            AgeGroupIds = updateTeacherDto.AgeGroupIds,
            CommunicationMethodIds = updateTeacherDto.CommunicationMethodIds,
            TeachingLanguageIds = updateTeacherDto.TeachingLanguageIds
        };

        // Send command to handler
        var result = await mediator.Send(command);
        var resultModel = result;

        return result.IsSuccess
            ? Results.Ok(resultModel)
            : Results.BadRequest(resultModel);
    }
} 