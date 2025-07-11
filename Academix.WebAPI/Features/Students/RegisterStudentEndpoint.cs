using Academix.Application.Features.Students.Commands.RegisterStudent;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Helpers;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academix.WebAPI.Features.Students
{
    public class RegisterStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/students/register", HandleAsync)
                .WithName("RegisterStudent")
                .WithTags("Students")
                .Accepts<RegisterStudentCommand>("application/json")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] RegisterStudentCommand command,
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