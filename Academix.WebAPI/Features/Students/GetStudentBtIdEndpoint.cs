using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Students.Query.GetById;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class GetStudentBtIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/student/{id:guid}", HandleAsync)
                .WithName("GetStudentById")
                .WithTags("Students")
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
                var result = await mediator.Send(new GetStudentByIdQuery { Id = id }, cancellationToken);

                if (result.IsSuccess && result.Value is not null)
                    return Results.Ok(response.Success(result.Value));

                return Results.NotFound(response.BadRequest(result.Error));
            }
            catch (Exception)
            {
                var message = localizationService.GetLocalizedString("StudentGetByIdFailed");
                return Results.BadRequest(response.BadRequest(message));
            }
        }
    }

}
