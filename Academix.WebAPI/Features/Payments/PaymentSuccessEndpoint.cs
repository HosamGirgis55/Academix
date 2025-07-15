using Academix.Application.Common.Interfaces;
using Academix.Domain.Interfaces;
using Academix.Helpers;
using Academix.WebAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Payments;

public class PaymentSuccessEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/success", HandlePaymentSuccessAsync)
            .WithName("PaymentSuccess")
            .WithTags("Payments")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces<object>(404);
    }

    private static async Task<IResult> HandlePaymentSuccessAsync(
        [FromQuery] string? token,
        [FromQuery] string? PayerID,
        IPaymentRepository paymentRepository,
        IPayPalService payPalService,
        IPointsService pointsService,
        ResponseHelper responseHelper,
        ILogger<PaymentSuccessEndpoint> logger)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Payment success called without token parameter");
                return Results.BadRequest(responseHelper.BadRequest("Missing payment token"));
            }

            // Find payment by PayPal order ID
            var payment = await paymentRepository.GetByPayPalOrderIdAsync(token);
            if (payment == null)
            {
                logger.LogWarning("Payment not found for PayPal order ID: {OrderId}", token);
                return Results.NotFound(responseHelper.BadRequest("Payment not found"));
            }

            // Check if payment is already completed
            if (payment.Status == "Completed")
            {
                logger.LogInformation("Payment {PaymentId} already completed", payment.Id);
                return Results.Ok(responseHelper.Success(new
                {
                    message = "Payment already completed",
                    paymentId = payment.Id,
                    pointsAdded = payment.PointsAmount
                }));
            }

            // Get PayPal order status
            var payPalOrder = await payPalService.GetOrderDetailsAsync(token);
            
            if (payPalOrder?.Status == "APPROVED")
            {
                // Capture the payment
                var captureResult = await payPalService.CaptureOrderAsync(token);
                
                if (captureResult.IsSuccess)
                {
                    // Update payment status
                    payment.Status = "Completed";
                    payment.CompletedAt = DateTime.UtcNow;
                    payment.PayPalTransactionId = captureResult.TransactionId;
                    payment.PayPalPayerId = PayerID;
                    
                    await paymentRepository.UpdateAsync(payment);

                    // Add points to user
                    var pointsResult = await pointsService.AddPointsToUserAsync(payment.UserId, payment.PointsAmount);
                    
                    if (pointsResult)
                    {
                        logger.LogInformation("Successfully processed payment {PaymentId} and added {Points} points to user {UserId}", 
                            payment.Id, payment.PointsAmount, payment.UserId);
                        
                        return Results.Ok(responseHelper.Success(new
                        {
                            message = "Payment completed successfully",
                            paymentId = payment.Id,
                            pointsAdded = payment.PointsAmount,
                            transactionId = captureResult.TransactionId
                        }));
                    }
                    else
                    {
                        logger.LogWarning("Payment captured but failed to add points to user {UserId}. User may not be registered as student or teacher yet.", 
                            payment.UserId);
                        
                        return Results.Ok(responseHelper.Success(new
                        {
                            message = "Payment completed but points addition failed - please complete your profile registration",
                            paymentId = payment.Id,
                            transactionId = captureResult.TransactionId,
                            note = "Points will be added once you register as a student or teacher"
                        }));
                    }
                }
                else
                {
                    logger.LogError("Failed to capture PayPal payment {OrderId}: {Error}", token, captureResult.ErrorMessage);
                    
                    payment.Status = "Failed";
                    payment.FailedAt = DateTime.UtcNow;
                    payment.FailureReason = captureResult.ErrorMessage;
                    await paymentRepository.UpdateAsync(payment);
                    
                    return Results.BadRequest(responseHelper.BadRequest("Payment capture failed: " + captureResult.ErrorMessage));
                }
            }
            else
            {
                logger.LogWarning("Payment {OrderId} not approved. Status: {Status}", token, payPalOrder?.Status);
                return Results.BadRequest(responseHelper.BadRequest($"Payment not approved. Status: {payPalOrder?.Status}"));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment success for token: {Token}", token);
            return Results.Problem("An error occurred while processing payment");
        }
    }
} 