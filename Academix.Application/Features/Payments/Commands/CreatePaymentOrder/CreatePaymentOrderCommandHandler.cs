using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academix.Application.Features.Payments.Commands.CreatePaymentOrder
{
    public class CreatePaymentOrderCommandHandler : IRequestHandler<CreatePaymentOrderCommand, Result<CreatePaymentOrderResult>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPayPalService _payPalService;
        private readonly ILogger<CreatePaymentOrderCommandHandler> _logger;
        private readonly ILocalizationService _localizationService;

        public CreatePaymentOrderCommandHandler(
            IPaymentRepository paymentRepository,
            IPayPalService payPalService,
            ILogger<CreatePaymentOrderCommandHandler> logger,
            ILocalizationService localizationService)
        {
            _paymentRepository = paymentRepository;
            _payPalService = payPalService;
            _logger = logger;
            _localizationService = localizationService;
        }

        public async Task<Result<CreatePaymentOrderResult>> Handle(CreatePaymentOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate points amount
                if (request.PointsAmount <= 0)
                {
                    var pointsErrorMessage = _localizationService.GetLocalizedString("InvalidPointsAmount");
                    return Result<CreatePaymentOrderResult>.Failure(pointsErrorMessage ?? "Invalid points amount");
                }

                if (request.PointPrice <= 0)
                {
                    var priceErrorMessage = _localizationService.GetLocalizedString("InvalidPointPrice");
                    return Result<CreatePaymentOrderResult>.Failure(priceErrorMessage ?? "Invalid point price");
                }

                // Create PayPal order
                var payPalResult = await _payPalService.CreateOrderAsync(
                    request.Amount, 
                    request.Currency, 
                    request.Description);

                if (!payPalResult.IsSuccess || string.IsNullOrEmpty(payPalResult.OrderId))
                {
                    var paypalErrorMessage = _localizationService.GetLocalizedString("PayPalOrderCreationFailed");
                    return Result<CreatePaymentOrderResult>.Failure(paypalErrorMessage ?? "PayPal order creation failed");
                }

                // Create payment entity
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Status = "Pending",
                    PayPalOrderId = payPalResult.OrderId,
                    Description = request.Description ?? $"Purchase of {request.PointsAmount} points",
                    Reference = request.Reference,
                    PointsAmount = request.PointsAmount,
                    PointPrice = request.PointPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId
                };

                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Payment order created successfully. PaymentId: {PaymentId}, PayPalOrderId: {PayPalOrderId}", 
                    payment.Id, payPalResult.OrderId);

                var result = new CreatePaymentOrderResult
                {
                    PaymentId = payment.Id,
                    PayPalOrderId = payPalResult.OrderId,
                    PaymentUrl = payPalResult.ApprovalUrl ?? "",
                    CreatedAt = payment.CreatedAt,
                    PointsAmount = payment.PointsAmount,
                    PointPrice = payment.PointPrice,
                    TotalAmount = payment.Amount
                };

                return Result<CreatePaymentOrderResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment order for user {UserId}", request.UserId);
                var errorMessage = _localizationService.GetLocalizedString("PaymentCreationFailed");
                return Result<CreatePaymentOrderResult>.Failure($"{errorMessage ?? "Payment creation failed"}: {ex.Message}");
            }
        }
    }
} 