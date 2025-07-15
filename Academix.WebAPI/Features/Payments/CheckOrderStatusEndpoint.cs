using Academix.Application.Common.Interfaces;
using Academix.Domain.Interfaces;
using Academix.Helpers;
using Academix.WebAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Payments;

public class CheckOrderStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/check-order/{orderId}", CheckOrderStatusAsync)
            .WithName("CheckOrderStatus")
            .WithTags("Payments")
            .Produces<object>(200)
            .Produces<object>(400)
            .Produces<object>(404)
            .WithSummary("Check PayPal order status")
            .WithDescription("Checks the status of a PayPal order and local payment record");
    }

    private static async Task<IResult> CheckOrderStatusAsync(
        HttpContext httpContext,
        IPayPalService payPalService,
        IPaymentRepository paymentRepository,
        string orderId)
    {
        try
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";

            // Get local payment record
            var payment = await paymentRepository.GetByPayPalOrderIdAsync(orderId);
            if (payment == null)
            {
                var notFoundMessage = culture == "ar" ? "لم يتم العثور على أمر الدفع" : "Payment order not found";
                return Results.Ok(new ResponseHelper()
                    .BadRequest(notFoundMessage, culture));
            }

            // Get PayPal order details
            var payPalOrder = await payPalService.GetOrderDetailsAsync(orderId);

            var result = new
            {
                PaymentId = payment.Id,
                PayPalOrderId = payment.PayPalOrderId,
                LocalStatus = payment.Status,
                PayPalStatus = payPalOrder.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Description = payment.Description,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt,
                TransactionId = payment.PayPalTransactionId
            };

            var successMessage = culture == "ar" ? "تم استرداد حالة الطلب بنجاح" : "Order status retrieved successfully";
            return Results.Ok(new ResponseHelper()
                .Success(result, culture)
                .WithMassage(successMessage));
        }
        catch (Exception ex)
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
            var errorMessage = culture == "ar" ? "حدث خطأ غير متوقع أثناء فحص حالة الطلب" : "An unexpected error occurred while checking order status";
            
            return Results.Ok(new ResponseHelper()
                .ServerError($"{errorMessage}: {ex.Message}", culture));
        }
    }
} 