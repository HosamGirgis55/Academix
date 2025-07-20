namespace Academix.Application.Common.Interfaces
{
    public interface IPayPalService
    {
        Task<PayPalCreateOrderResult> CreateOrderAsync(decimal amount, string currency = "USD", string? description = null, string succesUrl = "", string cancleUrl = "");
        Task<PayPalCaptureResult> CaptureOrderAsync(string orderId);
        Task<PayPalOrderDetails> GetOrderDetailsAsync(string orderId);
    }

    public class PayPalCreateOrderResult
    {
        public bool IsSuccess { get; set; }
        public string? OrderId { get; set; }
        public string? ApprovalUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayPalCaptureResult
    {
        public bool IsSuccess { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentId { get; set; }
        public decimal? CapturedAmount { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? CapturedAt { get; set; }
    }

    public class PayPalOrderDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? PayerId { get; set; }
        public string? PayerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 