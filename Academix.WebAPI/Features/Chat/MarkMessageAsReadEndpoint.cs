using Academix.Application.Features.Chat.Commands.MakeMessageAsReadCommand;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Chat
{
    public class MarkMessageAsReadEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/chat/messages/mark-as-read", HandleAsync)
                .WithName("MarkMessagesAsRead")
                .WithTags("Chat")
                .Accepts<MarkMessageAsReadCommand>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] MarkMessageAsReadCommand command,
            [FromServices] ISender sender,
            [FromServices] ResponseHelper response,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(response.BadRequest(result.Error));

                return Results.Ok(response.Success());
            }
            catch (Exception ex)
            {
                response.ServerError(ex.Message);
                return Results.Problem(title: "Internal Server Error", detail: ex.Message);
            }
        }
    }

}
