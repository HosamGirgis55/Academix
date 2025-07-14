using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Teachers.Query.GetById;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Teachers
{
    public class GetTeacherById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/teachers/{id:guid}", HandleAsync)
                .WithName("GetTeacherById")
                .WithTags("Teachers")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(404)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await mediator.Send(new GetTeacherByIdQuery { Id = id }, cancellationToken);

                if (result.IsSuccess && result.Value is not null)
                    return Results.Ok(response.Success(result.Value));

                return Results.NotFound(response.BadRequest(result.Error));
            }
            catch (Exception)
            {
                var message = localizationService.GetLocalizedString("TeacherGetByIdFailed");
                return Results.BadRequest(response.BadRequest(message));
            }
        }
    }
}
