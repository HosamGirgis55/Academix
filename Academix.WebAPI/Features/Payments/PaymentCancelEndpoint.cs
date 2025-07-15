using Academix.Domain.Interfaces;
using Academix.Helpers;
using Academix.WebAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Payments;

public class PaymentCancelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/cancel", HandlePaymentCancelAsync)
            .WithName("PaymentCancel")
            .WithTags("Payments")
            .Produces<object>(200)
            .Produces<object>(404);
    }

    private static async Task<IResult> HandlePaymentCancelAsync(
        [FromQuery] string? token,
        IPaymentRepository paymentRepository,
        ResponseHelper responseHelper,
        ILogger<PaymentCancelEndpoint> logger)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Payment cancel called without token parameter");
                return Results.Ok(responseHelper.Success(new
                {
                    message = "Payment was cancelled",
                    status = "cancelled"
                }));
            }

            // Find payment by PayPal order ID
            var payment = await paymentRepository.GetByPayPalOrderIdAsync(token);
            if (payment != null)
            {
                // Update payment status to cancelled if it's still pending
                if (payment.Status == "Pending" || payment.Status == "Created")
                {
                    payment.Status = "Cancelled";
                    payment.FailedAt = DateTime.UtcNow;
                    payment.FailureReason = "Payment cancelled by user";
                    
                    await paymentRepository.UpdateAsync(payment);
                    
                    logger.LogInformation("Payment {PaymentId} cancelled by user", payment.Id);
                }
                
                return Results.Ok(responseHelper.Success(new
                {
                    message = "Payment was cancelled",
                    paymentId = payment.Id,
                    status = "cancelled"
                }));
            }
            else
            {
                logger.LogWarning("Payment not found for cancelled PayPal order ID: {OrderId}", token);
                return Results.Ok(responseHelper.Success(new
                {
                    message = "Payment was cancelled",
                    status = "cancelled"
                }));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment cancellation for token: {Token}", token);
            return Results.Ok(responseHelper.Success(new
            {
                message = "Payment was cancelled",
                status = "cancelled"
            }));
        }
    }
} 