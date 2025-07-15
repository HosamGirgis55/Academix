using Academix.Domain.Entities;

namespace Academix.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByPayPalOrderIdAsync(string payPalOrderId);
        Task<List<Payment>> GetByUserIdAsync(string userId);
        Task<Payment> UpdateAsync(Payment payment);
        Task SaveChangesAsync();
    }
} 