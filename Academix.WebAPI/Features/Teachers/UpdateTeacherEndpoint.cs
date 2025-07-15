using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Commands.UpdateTeacher;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

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
        HttpContext httpContext,
        IMediator mediator)
    {
        try
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

            // Read and validate request body
            UpdateTeacherDto? updateTeacherDto;
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body);
                var requestBody = await reader.ReadToEndAsync();
                
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return Results.BadRequest(new ResultModel
                    {
                        Success = false,
                        Message = "Request body cannot be empty. Please provide JSON data to update.",
                        Errors = new List<string> { "Empty request body" }
                    });
                }

                updateTeacherDto = JsonSerializer.Deserialize<UpdateTeacherDto>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                });

                if (updateTeacherDto == null)
                {
                    return Results.BadRequest(new ResultModel
                    {
                        Success = false,
                        Message = "Invalid JSON format. Please check your request body.",
                        Errors = new List<string> { "Failed to deserialize JSON" }
                    });
                }
            }
            catch (JsonException ex)
            {
                return Results.BadRequest(new ResultModel
                {
                    Success = false,
                    Message = "Invalid JSON format in request body.",
                    Errors = new List<string> { $"JSON parsing error: {ex.Message}" }
                });
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
        catch (Exception ex)
        {
            return Results.BadRequest(new ResultModel
            {
                Success = false,
                Message = "An unexpected error occurred while updating teacher profile.",
                Errors = new List<string> { ex.Message }
            });
        }
    }
} 