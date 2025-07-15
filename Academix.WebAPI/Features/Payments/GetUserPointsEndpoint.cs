using Academix.Application.Common.Interfaces;
using Academix.Helpers;
using Academix.WebAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Payments;

public class GetUserPointsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/user-points", GetUserPointsAsync)
            .WithName("GetUserPoints")
            .WithTags("Payments")
            .RequireAuthorization()
            .Produces<object>(200)
            .Produces(401)
            .WithSummary("Get user points balance")
            .WithDescription("Get the current points balance for the authenticated user");
    }

    private static async Task<IResult> GetUserPointsAsync(
        HttpContext httpContext,
        IPointsService pointsService)
    {
        try
        {
            // Get user ID from JWT token
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";

            var points = await pointsService.GetUserPointsAsync(userId);

            var result = new
            {
                UserId = userId,
                Points = points,
                RetrievedAt = DateTime.UtcNow
            };

            var successMessage = culture == "ar" ? "تم جلب رصيد النقاط بنجاح" : "Points balance retrieved successfully";
            return Results.Ok(new ResponseHelper()
                .Success(result, culture)
                .WithMassage(successMessage));
        }
        catch (Exception ex)
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
            var errorMessage = culture == "ar" ? "حدث خطأ غير متوقع أثناء جلب رصيد النقاط" : "An unexpected error occurred while retrieving points balance";
            
            return Results.Ok(new ResponseHelper()
                .ServerError($"{errorMessage}: {ex.Message}", culture));
        }
    }
} 