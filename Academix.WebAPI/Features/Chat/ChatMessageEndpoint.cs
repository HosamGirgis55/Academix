using Academix.Application.Features.Chat.Queries;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Chat
{
    public class GetChatMessagesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/chat/messages", HandleAsync)
                .WithName("GetChatMessages")
                .WithTags("Chat")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromQuery] string currentUserId,
            [FromQuery] string otherUserId,
            [FromServices] ISender sender,
            [FromServices] ResponseHelper response,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await sender.Send(new GetChatMessageQuery
                {
                    CurrentUserId = currentUserId,
                    OtherUserId = otherUserId
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
