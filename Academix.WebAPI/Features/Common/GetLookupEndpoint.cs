using Academix.Application.Common.Models;
using Academix.Application.Features.Common.Queries.GetLookup;
using Academix.Domain.DTOs;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Academix.WebAPI.Features.Common;

public class GetLookupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/lookup/{type}", async (string type, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetLookupQuery { Type = type };
            var result = await mediator.Send(query, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result);
        })
        .WithName("GetLookup")
        .WithTags("Lookup")
        .Produces<Result<List<LookupItemDto>>>(StatusCodes.Status200OK)
        .Produces<Result<List<LookupItemDto>>>(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
} 