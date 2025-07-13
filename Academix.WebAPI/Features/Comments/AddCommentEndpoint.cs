using Academix.Application.Features.Comments.Commands.AddComment;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Helpers;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Academix.WebAPI.Features.Comments
{
    public class AddCommentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/comments", HandleAsync)
                .WithName("AddComment")
                .WithTags("Comments")
                .Accepts<AddCommentCommand>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] AddCommentCommand command,
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(response.Success(result.SuccessMessage));
                }

                return Results.BadRequest(response.BadRequest(result.Error));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(response.BadRequest(localizationService.GetLocalizedString("CommentAddFailed")));
            }
        }
    }
} 