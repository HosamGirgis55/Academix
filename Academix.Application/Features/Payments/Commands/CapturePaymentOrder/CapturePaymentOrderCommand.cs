using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Payments.Commands.CapturePaymentOrder
{
    public class CapturePaymentOrderCommand : IRequest<Result<CapturePaymentOrderResult>>
    {
        public string PayPalOrderId { get; set; } = string.Empty;
        public string? PayerId { get; set; }
    }

    public class CapturePaymentOrderResult
    {
        public Guid PaymentId { get; set; }
        public string PayPalTransactionId { get; set; } = string.Empty;
        public decimal CapturedAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CapturedAt { get; set; }
        public bool IsSuccess { get; set; }
    }
} 