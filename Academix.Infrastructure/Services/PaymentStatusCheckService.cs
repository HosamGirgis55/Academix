using Academix.Application.Common.Interfaces;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Academix.Infrastructure.Services
{
    public class PaymentStatusCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentStatusCheckService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2); // Check every 2 minutes

        public PaymentStatusCheckService(
            IServiceProvider serviceProvider,
            ILogger<PaymentStatusCheckService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment Status Check Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckPendingPaymentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking payment status");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckPendingPaymentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            var payPalService = scope.ServiceProvider.GetRequiredService<IPayPalService>();
            var pointsService = scope.ServiceProvider.GetRequiredService<IPointsService>();

            try
            {
                // Get all pending payments
                var pendingPayments = await paymentRepository.GetPendingPaymentsAsync();
                
                _logger.LogInformation("Checking {Count} pending payments", pendingPayments.Count());

                foreach (var payment in pendingPayments)
                {
                    try
                    {
                        await CheckAndUpdatePaymentStatusAsync(payment, payPalService, pointsService, paymentRepository);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error checking payment {PaymentId}", payment.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending payments");
            }
        }

        private async Task CheckAndUpdatePaymentStatusAsync(
            Payment payment, 
            IPayPalService payPalService, 
            IPointsService pointsService,
            IPaymentRepository paymentRepository)
        {
            try
            {
                // Check PayPal order status
                var orderDetails = await payPalService.GetOrderDetailsAsync(payment.PayPalOrderId ?? "");
                
                _logger.LogInformation("Checking payment {PaymentId}, PayPal status: {Status}", 
                    payment.Id, orderDetails.Status);

                // If payment is approved, capture it and add points
                if (orderDetails.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase))
                {
                    // Capture the payment
                    var captureResult = await payPalService.CaptureOrderAsync(payment.PayPalOrderId ?? "");
                    
                    if (captureResult.IsSuccess)
                    {
                        // Update payment status
                        payment.Status = "Completed";
                        payment.ProcessedAt = DateTime.UtcNow;
                        payment.PayPalTransactionId = captureResult.TransactionId;
                        payment.UpdatedAt = DateTime.UtcNow;

                        await paymentRepository.UpdateAsync(payment);

                        // Add points to user account
                        var pointsAdded = await pointsService.AddPointsToUserAsync(payment.UserId, payment.PointsAmount);
                        
                        if (pointsAdded)
                        {
                            _logger.LogInformation("Successfully added {Points} points to user {UserId} for payment {PaymentId}",
                                payment.PointsAmount, payment.UserId, payment.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to add points to user {UserId} for payment {PaymentId}",
                                payment.UserId, payment.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to capture PayPal payment {PayPalOrderId}: {Error}",
                            payment.PayPalOrderId, captureResult.ErrorMessage);
                    }
                }
                else if (orderDetails.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    // Payment already completed, just update status and add points if not already done
                    if (payment.Status != "Completed")
                    {
                        payment.Status = "Completed";
                        payment.ProcessedAt = DateTime.UtcNow;
                        payment.UpdatedAt = DateTime.UtcNow;

                        await paymentRepository.UpdateAsync(payment);

                        // Add points to user account
                        var pointsAdded = await pointsService.AddPointsToUserAsync(payment.UserId, payment.PointsAmount);
                        
                        if (pointsAdded)
                        {
                            _logger.LogInformation("Successfully added {Points} points to user {UserId} for completed payment {PaymentId}",
                                payment.PointsAmount, payment.UserId, payment.Id);
                        }
                    }
                }
                else if (orderDetails.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase) ||
                         orderDetails.Status.Equals("VOIDED", StringComparison.OrdinalIgnoreCase))
                {
                    // Payment was cancelled
                    payment.Status = "Cancelled";
                    payment.UpdatedAt = DateTime.UtcNow;
                    await paymentRepository.UpdateAsync(payment);

                    _logger.LogInformation("Payment {PaymentId} was cancelled", payment.Id);
                }
                // For other statuses (CREATED, SAVED, PAYER_ACTION_REQUIRED), keep checking
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking PayPal order status for payment {PaymentId}", payment.Id);
            }
        }
    }
} 