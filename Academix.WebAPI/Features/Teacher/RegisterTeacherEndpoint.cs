using Academix.Application.Features.Students.Commands.RegisterStudent;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Application.Features.Teacher.Commands.RegisterStudent;

namespace Academix.WebAPI.Features.Teacher
{
    public class RegisterTeacherEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Teacher/register", HandleAsync)
                .WithName("RegisterTeacher")
                .WithTags("Teacher")
                .Produces<ResultModel<TeacherRegistrationResponse>>(200)
                .Produces<ResultModel<TeacherRegistrationResponse>>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] RegisterTeacherCommand command,
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