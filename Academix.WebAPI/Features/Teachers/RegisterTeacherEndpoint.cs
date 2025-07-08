using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Commands.RegisterTeacher;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Teachers
{
    public class RegisterTeacherEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/teachers/register", HandleAsync)
                .WithName("RegisterTeacher")
                .WithTags("Teachers")
                .Produces<ResultModel<AuthenticationResult>>(200)
                .Produces<ResultModel<AuthenticationResult>>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] RegisterTeacherCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            var resultModel = result.ToResultModel(result.SuccessMessage);

            if (resultModel.Success)
            {
                return Results.Ok(resultModel);
            }

            return Results.BadRequest(resultModel);
        }
    }
} 