using Academix.Application.Features.Sessions.Commands.AcceptSessionRequest;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Helpers;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Sessions
{
    public class AcceptSessionRequestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/sessions/accept", HandleAsync)
                .WithName("AcceptSessionRequest")
                .WithTags("Sessions")
                .Accepts<AcceptSessionRequestDto>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] AcceptSessionRequestDto dto,
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get the authenticated user's ID
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                // Create command
                var command = new AcceptSessionRequestCommand
                {
                    SessionRequestId = dto.SessionRequestId,
                    TeacherId = dto.TeacherId,
                    ScheduledStartTime = dto.ScheduledStartTime
                };

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(response.Success(new { SessionId = result.Value }, 
                        result.SuccessMessage ?? localizationService.GetLocalizedString("SessionRequestAcceptedSuccessfully")));
                }

                return Results.BadRequest(response.BadRequest(result.Error ?? localizationService.GetLocalizedString("AcceptSessionRequestFailed")));
            }
            catch (Exception ex)
            {
                var message = localizationService.GetLocalizedString("AcceptSessionRequestFailed");
                return Results.BadRequest(response.BadRequest($"{message}: {ex.Message}"));
            }
        }
    }

    public class AcceptSessionRequestDto
    {
        public Guid SessionRequestId { get; set; }
        public Guid TeacherId { get; set; }
        public DateTime ScheduledStartTime { get; set; }
    }
} 