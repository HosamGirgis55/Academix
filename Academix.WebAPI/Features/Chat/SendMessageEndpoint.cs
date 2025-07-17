using Academix.Application.Features.Chat.Commands.SentMessage;
using Academix.Application.Features.Chat.Queries;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Chat
{
    public class SendMessageEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/chat/sendMessage", HandleAsync)
                .WithName("SendMessage")
                .WithTags("Chat")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);

        }

        private static async Task<IResult> HandleAsync(
            [FromQuery] string currentUserId,
            [FromQuery] string otherUserId,
            [FromQuery] string message,
            [FromServices] ISender sender,
            [FromServices] ResponseHelper response,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await sender.Send(new SendMessageCommand
                {
                    SenderId = currentUserId,
                    ReceiverId = otherUserId,
                    MessageText = message
                }, cancellationToken);

                return Results.Ok(response.Success(result));
            }
            catch (Exception ex)
            {
                response.ServerError(ex.Message);
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

        }
    }
}
