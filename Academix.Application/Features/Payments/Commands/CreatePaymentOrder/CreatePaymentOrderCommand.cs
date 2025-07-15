using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Payments.Commands.CreatePaymentOrder
{
    public class CreatePaymentOrderCommand : IRequest<Result<CreatePaymentOrderResult>>
    {
        public string UserId { get; set; } = string.Empty;
        public int PointsAmount { get; set; }
        public decimal PointPrice { get; set; } = 0.01m; // Default: $0.01 per point
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
        public string? Reference { get; set; }
        
        // Calculated property
        public decimal Amount => PointsAmount * PointPrice;
    }

    public class CreatePaymentOrderResult
    {
        public Guid PaymentId { get; set; }
        public string PayPalOrderId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PointsAmount { get; set; }
        public decimal PointPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
} 