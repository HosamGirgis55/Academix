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
        HttpContext httpContext,
        IMediator mediator,
        [FromBody] CreatePaymentOrderRequest request)
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

            var command = new CreatePaymentOrderCommand
            {
                UserId = userId,
                PointsAmount = request.PointsAmount,
                PointPrice = request.PointPrice,
                Currency = request.Currency ?? "USD",
                Description = request.Description,
                Reference = request.Reference
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                var successMessage = culture == "ar" ? "تم إنشاء أمر الدفع بنجاح" : "Payment order created successfully";
                return Results.Ok(new ResponseHelper()
                    .Success(result.Value, culture)
                    .WithMassage(successMessage));
            }
            else
            {
                return Results.Ok(new ResponseHelper()
                    .BadRequest(result.Error, culture));
            }
        }
        catch (Exception ex)
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
            var errorMessage = culture == "ar" ? "حدث خطأ غير متوقع أثناء إنشاء أمر الدفع" : "An unexpected error occurred while creating payment order";
            
            return Results.Ok(new ResponseHelper()
                .ServerError($"{errorMessage}: {ex.Message}", culture));
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