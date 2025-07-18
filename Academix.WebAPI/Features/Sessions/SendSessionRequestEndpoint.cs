using Academix.Application.Features.Sessions.Commands.SendSessionRequest;
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
    public class SendSessionRequestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/sessions/request", HandleAsync)
                .WithName("SendSessionRequest")
                .WithTags("Sessions")
                .Accepts<SendSessionRequestDto>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
                //.RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] SendSessionRequestDto dto,
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
                //if (string.IsNullOrEmpty(userId))
                //{
                //    return Results.Unauthorized();
                //}

                // Create command
                var command = new SendSessionRequestCommand
                {
                    StudentId = dto.StudentId,
                    TeacherId = dto.TeacherId,
                    PointsAmount = dto.PointsAmount,
                    Subject = dto.Subject,
                    Description = dto.Description,
                    EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
                    RequestedDateTime = dto.RequestedDateTime
                };

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(response.Success(new { SessionRequestId = result.Value }, 
                        result.SuccessMessage ?? localizationService.GetLocalizedString("SessionRequestSentSuccessfully")));
                }

                return Results.BadRequest(response.BadRequest(result.Error ?? localizationService.GetLocalizedString("SessionRequestFailed")));
            }
            catch (Exception ex)
            {
                var message = localizationService.GetLocalizedString("SessionRequestFailed");
                return Results.BadRequest(response.BadRequest($"{message}: {ex.Message}"));
            }
        }
    }

    public class SendSessionRequestDto
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public int PointsAmount { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public DateTime RequestedDateTime { get; set; }
    }
} 