using Academix.Application.Features.Dashboard.Queries.GetSpecializationById;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.DashBoard;

public class GetSpecializationByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/specializations/{id:guid}", GetSpecializationByIdAsync)
            .WithName("GetSpecializationById")
            .WithTags("Dashboard")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> GetSpecializationByIdAsync(Guid id, IMediator mediator)
    {
        try
        {
            var query = new GetSpecializationByIdQuery { Id = id };
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Data = result.Value, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while retrieving the specialization.", Error = ex.Message });
        }
    }
} 