using Academix.Application.Features.Dashboard.Queries.GetAllSpecializations;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.DashBoard;

public class GetAllSpecializationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/specializations", GetAllSpecializationsAsync)
            .WithName("GetAllSpecializations")
            .WithTags("Dashboard")
         //   .RequireAuthorization()
            .Produces(200)
            .Produces(400);
    }

    private static async Task<IResult> GetAllSpecializationsAsync(IMediator mediator)
    {
        try
        {
            var query = new GetAllSpecializationsQuery();
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(new { Success = true, Data = result.Value, Message = result.SuccessMessage })
                : Results.BadRequest(new { Success = false, Message = result.Error, Errors = result.Errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Success = false, Message = "An error occurred while retrieving specializations.", Error = ex.Message });
        }
    }
} 