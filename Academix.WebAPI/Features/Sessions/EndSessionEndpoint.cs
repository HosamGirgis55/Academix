using Academix.Application.Features.Sessions.Commands.EndSession;
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
    public class EndSessionEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/sessions/{sessionId:guid}/end", HandleAsync)
                .WithName("EndSession")
                .WithTags("Sessions")
                .Accepts<EndSessionDto>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(
            Guid sessionId,
            [FromBody] EndSessionDto dto,
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
                var command = new EndSessionCommand
                {
                    SessionId = sessionId,
                    TeacherNotes = dto.TeacherNotes,
                    StudentNotes = dto.StudentNotes,
                    StudentRating = dto.StudentRating,
                    TeacherRating = dto.TeacherRating
                };

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(response.Success(
                        result.SuccessMessage ?? localizationService.GetLocalizedString("SessionEndedSuccessfully")));
                }

                return Results.BadRequest(response.BadRequest(result.Error ?? localizationService.GetLocalizedString("EndSessionFailed")));
            }
            catch (Exception ex)
            {
                var message = localizationService.GetLocalizedString("EndSessionFailed");
                return Results.BadRequest(response.BadRequest($"{message}: {ex.Message}"));
            }
        }
    }

    public class EndSessionDto
    {
        public string? TeacherNotes { get; set; }
        public string? StudentNotes { get; set; }
        public int? StudentRating { get; set; } // 1-5 stars
        public int? TeacherRating { get; set; } // 1-5 stars
    }
} 