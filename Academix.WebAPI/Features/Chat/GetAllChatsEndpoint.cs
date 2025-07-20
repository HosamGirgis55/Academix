using Academix.Application.Features.Chat.Queries.GetAllChats;
using Academix.Application.Features.Chat.Queries.GetChatMessage;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Chat
{
    public class GetAllChatsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/chat/AllChats", HandleAsync)
                .WithName("GetAllChats")
                .WithTags("Chat")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromQuery] string currentUserId,
            [FromServices] ISender sender,
            [FromServices] ResponseHelper response,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await sender.Send(new GetAllChatsQuery
                {
                    UserId = currentUserId
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
