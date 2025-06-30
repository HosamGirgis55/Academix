using Academix.Application.Features.Students.Commands.RegisterStudent;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Students
{
    public class RegisterStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/students/register", HandleAsync)
                .WithName("RegisterStudent")
                .WithTags("Students")
                .Produces<ResultModel<StudentRegistrationResponse>>(200)
                .Produces<ResultModel<StudentRegistrationResponse>>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] RegisterStudentCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            var resultModel = result.ToResultModel(result.Value?.Message);

            if (resultModel.Success)
            {
                return Results.Ok(resultModel);
            }

            return Results.BadRequest(resultModel);
        }
    }

    // No longer needed - using ResultModel<T> instead
} 