using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Dashboard.Commands.AddSkills;
using Academix.Application.Features.Dashboard.Commands.UpdateSpecialization;
using Academix.Application.Features.Dashboard.Commands.UpdatStatus;
using Academix.Domain.DTOs;
using Academix.Domain.Enums;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard
{
    public class UpdateStatusEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/skills/updateStatusTeacher/{id:guid}", HandleAsync)
                .WithName("UpdateTeacherStatus")
                .WithTags("Dashboard")
                .RequireAuthorization()
                .Produces(200)
                .Produces(400)
                .Produces(404);
        }

        private static async Task<IResult> HandleAsync(
            Guid id,
            [FromQuery] Status status,
            IMediator mediator)
        {
            try
            {
                var command = new UpdateStatusCommand
                {
                    Id = id,
                    Status = status
                };

                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Ok(new { Success = true, Message = result.SuccessMessage })
                    : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    Success = false,
                    Message = "An error occurred while updating the status.",
                    Error = ex.Message
                });
            }

        }
    }
}
