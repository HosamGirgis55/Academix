using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academix.Application.Features.Payments.Commands.CapturePaymentOrder
{
    public class CapturePaymentOrderCommandHandler : IRequestHandler<CapturePaymentOrderCommand, Result<CapturePaymentOrderResult>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPayPalService _payPalService;
        private readonly IPointsService _pointsService;
        private readonly ILogger<CapturePaymentOrderCommandHandler> _logger;
        private readonly ILocalizationService _localizationService;

        public CapturePaymentOrderCommandHandler(
            IPaymentRepository paymentRepository,
            IPayPalService payPalService,
            IPointsService pointsService,
            ILogger<CapturePaymentOrderCommandHandler> logger,
            ILocalizationService localizationService)
        {
            _paymentRepository = paymentRepository;
            _payPalService = payPalService;
            _pointsService = pointsService;
            _logger = logger;
            _localizationService = localizationService;
        }

        public async Task<Result<CapturePaymentOrderResult>> Handle(CapturePaymentOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the payment by PayPal order ID
                var payment = await _paymentRepository.GetByPayPalOrderIdAsync(request.PayPalOrderId);
                if (payment == null)
                {
                    var notFoundMessage = _localizationService.GetLocalizedString("PaymentNotFound");
                    return Result<CapturePaymentOrderResult>.Failure(notFoundMessage ?? "Payment not found");
                }

                // Check if payment is in correct status
                if (payment.Status != "Pending")
                {
                    var invalidStatusMessage = _localizationService.GetLocalizedString("PaymentInvalidStatus");
                    return Result<CapturePaymentOrderResult>.Failure(invalidStatusMessage ?? "Payment is not in pending status");
                }

                // Capture payment through PayPal
                var captureResult = await _payPalService.CaptureOrderAsync(request.PayPalOrderId);
                
                if (!captureResult.IsSuccess)
                {
                    payment.Status = "Failed";
                    payment.FailureReason = captureResult.ErrorMessage;
                    payment.FailedAt = DateTime.UtcNow;
                    payment.UpdatedAt = DateTime.UtcNow;

                    await _paymentRepository.UpdateAsync(payment);
                    await _paymentRepository.SaveChangesAsync();

                    _logger.LogWarning("Payment capture failed. PaymentId: {PaymentId}, Error: {Error}", 
                        payment.Id, captureResult.ErrorMessage);

                    var captureFailedMessage = _localizationService.GetLocalizedString("PaymentCaptureFailed");
                    return Result<CapturePaymentOrderResult>.Failure($"{captureFailedMessage ?? "Payment capture failed"}: {captureResult.ErrorMessage}");
                }

                // Update payment with capture details
                payment.Status = "Completed";
                payment.PayPalTransactionId = captureResult.TransactionId;
                payment.PayPalPaymentId = captureResult.PaymentId;
                payment.PayPalPayerId = request.PayerId;
                payment.ProcessedAt = DateTime.UtcNow;
                payment.CompletedAt = captureResult.CapturedAt ?? DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                
                // Calculate fees (PayPal typically charges 2.9% + $0.30 for domestic transactions)
                var feeAmount = Math.Round((payment.Amount * 0.029m) + 0.30m, 2);
                payment.FeeAmount = feeAmount;
                payment.NetAmount = payment.Amount - feeAmount;

                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                // Add points to user after successful payment
                var pointsAdded = await _pointsService.AddPointsToUserAsync(payment.UserId, payment.PointsAmount);
                if (!pointsAdded)
                {
                    _logger.LogWarning("Failed to add points to user {UserId} for payment {PaymentId}", 
                        payment.UserId, payment.Id);
                }
                else
                {
                    _logger.LogInformation("Successfully added {PointsAmount} points to user {UserId} for payment {PaymentId}", 
                        payment.PointsAmount, payment.UserId, payment.Id);
                }

                _logger.LogInformation("Payment captured successfully. PaymentId: {PaymentId}, TransactionId: {TransactionId}", 
                    payment.Id, captureResult.TransactionId);

                var result = new CapturePaymentOrderResult
                {
                    PaymentId = payment.Id,
                    PayPalTransactionId = captureResult.TransactionId ?? "",
                    CapturedAmount = captureResult.CapturedAmount ?? payment.Amount,
                    Status = "Completed",
                    CapturedAt = payment.CompletedAt.Value,
                    IsSuccess = true
                };

                return Result<CapturePaymentOrderResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing payment for PayPal order {PayPalOrderId}", request.PayPalOrderId);
                var errorMessage = _localizationService.GetLocalizedString("PaymentCaptureFailed");
                return Result<CapturePaymentOrderResult>.Failure($"{errorMessage ?? "Payment capture failed"}: {ex.Message}");
            }
        }
    }
} 