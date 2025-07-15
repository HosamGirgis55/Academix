using Academix.Application.Features.Payments.Commands.CapturePaymentOrder;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Payments;

public class CapturePaymentOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/capture-order", CapturePaymentOrderAsync)
            .WithName("CapturePaymentOrder")
            .WithTags("Payments")
            .Accepts<CapturePaymentOrderRequest>("application/json")
            .Produces<object>(200)
            .Produces<object>(400)
            .WithSummary("Capture a PayPal payment order")
            .WithDescription("Captures a PayPal order after user completes payment on PayPal");
    }

    private static async Task<IResult> CapturePaymentOrderAsync(
        HttpContext httpContext,
        IMediator mediator,
        [FromBody] CapturePaymentOrderRequest request)
    {
        try
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";

            var command = new CapturePaymentOrderCommand
            {
                PayPalOrderId = request.PayPalOrderId,
                PayerId = request.PayerId
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                var successMessage = culture == "ar" ? "تم تأكيد الدفع بنجاح" : "Payment captured successfully";
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
            var errorMessage = culture == "ar" ? "حدث خطأ غير متوقع أثناء تأكيد الدفع" : "An unexpected error occurred while capturing payment";
            
            return Results.Ok(new ResponseHelper()
                .ServerError($"{errorMessage}: {ex.Message}", culture));
        }
    }
}

public class CapturePaymentOrderRequest
{
    public string PayPalOrderId { get; set; } = string.Empty;
    public string? PayerId { get; set; }
} 