using Academix.Application.Features.Teachers.Commands.RegisterTeacher;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Helpers;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academix.WebAPI.Features.Teachers
{
    public class RegisterTeacherEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/teachers/register", HandleAsync)
                .WithName("RegisterTeacher")
                .WithTags("Teachers")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromBody] RegisterTeacherCommand command,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok();
            }

            return Results.BadRequest();
        }
    }
} 