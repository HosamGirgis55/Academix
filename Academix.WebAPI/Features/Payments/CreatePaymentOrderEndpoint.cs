using Academix.Application.Features.Payments.Commands.CreatePaymentOrder;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.WebAPI.Features.Payments;

public class CreatePaymentOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/create-order", CreatePaymentOrderAsync)
            .WithName("CreatePaymentOrder")
            .WithTags("Payments")
            .RequireAuthorization()
            .Accepts<CreatePaymentOrderRequest>("application/json")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces(401)
            .WithSummary("Create a PayPal payment order for points purchase")
            .WithDescription("Creates a PayPal order for purchasing points and returns payment URL for user to complete payment");
    }

    private static async Task<IResult> CreatePaymentOrderAsync(
        [FromBody] CreatePaymentOrderRequest request,
        [FromServices] IMediator mediator,
        [FromServices] ResponseHelper response,
        HttpContext httpContext)
    {
        try
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";

            // Ensure currency is always USD (fix "USA" to "USD")
            var currency = "USD";
            if (!string.IsNullOrEmpty(request.Currency))
            {
                // Convert common incorrect currency codes to USD
                currency = request.Currency.ToUpperInvariant() switch
                {
                    "USA" => "USD",
                    "US" => "USD",
                    "DOLLAR" => "USD",
                    "DOLLARS" => "USD",
                    _ => request.Currency.ToUpperInvariant() == "USD" ? "USD" : "USD" // Default to USD for any invalid currency
                };
            }

            var command = new CreatePaymentOrderCommand
            {
                UserId = userId,
                PointsAmount = request.PointsAmount,
                PointPrice = request.PointPrice,
                Currency = currency, // Always use validated USD currency
                Description = request.Description,
                Reference = request.Reference
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok(response.Success(result.Value, culture));
            }

            return Results.BadRequest(response.BadRequest(result.Error ?? "Failed to create payment order", culture));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error"
            );
        }
    }
}

public class CreatePaymentOrderRequest
{
    public int PointsAmount { get; set; }
    public decimal PointPrice { get; set; } = 0.01m; // Default: $0.01 per point
    public string? Currency { get; set; } = "USD";
    public string? Description { get; set; }
    public string? Reference { get; set; }
} 