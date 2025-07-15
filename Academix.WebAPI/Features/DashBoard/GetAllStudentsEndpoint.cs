using Academix.Application.Common.Interfaces;
using Academix.Application.Features.Dashboard.Query.Student.GetAllStudents;
using Academix.Application.Features.Dashboard.Query.Teacher.GetTeachers;
using Academix.Domain.Enums;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.DashBoard
{
    public class GetAllStudentsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/GetStudents", HandleAsync)
                .WithName("GetStudents")
                .WithTags("Dashboard")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400);
        }

        private static async Task<IResult> HandleAsync(
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllStudentQuery();

                var result = await mediator.Send(query, cancellationToken);

                if (result.IsSuccess)
                    return Results.Ok(response.Success(result.Value));

                return Results.BadRequest(response.BadRequest(result.Error));
            }
            catch (Exception)
            {
                var message = localizationService.GetLocalizedString("StudentGetAllFailed");
                return Results.BadRequest(response.BadRequest(message));
            }
        }
    }
}

