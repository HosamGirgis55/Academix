using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Dashboard.Commands.AddSkills;
using Academix.Application.Features.Students.Commands.RegisterStudent;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Academix.WebAPI.Features.DashBoard
{
    public class AddSkillsEndPoint : IEndpoint
    {

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/skills/add", HandleAsync)
                .WithName("AddSkilles")
                .WithTags("DashBoard")
                .Accepts<AddSkillsCommand>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] AddSkillsCommand command,
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
                    return Results.Ok(response.Created(result));
                }

                if (result.Errors.Any())
                {
                    var validationDictionary = new Dictionary<string, List<string>>
                    {
                        { "Validation", result.Errors }
                    };

                    response.WithValidation(validationDictionary);
                    return Results.BadRequest(response);
                }

                return Results.BadRequest(response.BadRequest(result.Error));
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

