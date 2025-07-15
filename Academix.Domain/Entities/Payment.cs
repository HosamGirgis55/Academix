using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class Payment : BaseEntity
    {
        // User Information
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        // Payment Details
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Cancelled
        public string? Description { get; set; }
        public string? Reference { get; set; }
        
        // Points Information
        public int PointsAmount { get; set; }
        public decimal PointPrice { get; set; } // Price per point in USD

        // PayPal Integration Fields
        public string? PayPalOrderId { get; set; }
        public string? PayPalPaymentId { get; set; }
        public string? PayPalPayerId { get; set; }
        public string? PayPalTransactionId { get; set; }

        // Timestamps
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? FailedAt { get; set; }

        // Failure Information
        public string? FailureReason { get; set; }

        // Fee Information
        public decimal? FeeAmount { get; set; }
        public decimal? NetAmount { get; set; }
    }
} 