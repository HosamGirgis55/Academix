using Academix.Application.Common.Models;
using Academix.Application.Features.Lookups.Queries.GetLookup;
using Academix.Domain.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Lookups;

public class GetLookupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/lookup", async (
            [FromQuery(Name = "type")] string type,
            [FromQuery(Name = "lang")] string? lang,
            ISender mediator,
            ILogger<GetLookupEndpoint> logger) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return Results.BadRequest(Result<List<LookupItemDto>>.Failure("Type parameter is required"));
                }

                if (!string.IsNullOrEmpty(lang) && !IsValidLanguage(lang))
                {
                    return Results.BadRequest(Result<List<LookupItemDto>>.Failure("Invalid language code. Use 'en' or 'ar'"));
                }

                var query = new GetLookupQuery
                {
                    Type = type,
                    Language = lang ?? "en"
                };

                logger.LogInformation("Processing lookup request for type: {Type}, language: {Language}", type, lang ?? "en");
                var result = await mediator.Send(query);
                
                return Results.Ok(Result<List<LookupItemDto>>.Success(result));
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid lookup request");
                return Results.BadRequest(Result<List<LookupItemDto>>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing lookup request");
                return Results.Problem(
                    title: "An unexpected error occurred while processing the lookup request",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        })
        .WithName("GetLookup")
        .WithSummary("Get lookup data for entities and enums")
        .WithTags("Lookups")
        .Produces<Result<List<LookupItemDto>>>(200)
        .Produces<Result<List<LookupItemDto>>>(400)
        .Produces<ProblemDetails>(500)
        .AllowAnonymous();
    }

    private static bool IsValidLanguage(string lang)
    {
        return string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(lang, "ar", StringComparison.OrdinalIgnoreCase);
    }
} 