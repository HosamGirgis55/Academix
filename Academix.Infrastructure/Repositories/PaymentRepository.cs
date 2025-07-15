using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Academix.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return payment;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByPayPalOrderIdAsync(string payPalOrderId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PayPalOrderId == payPalOrderId);
        }

        public async Task<List<Payment>> GetByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.Status == "Pending" || p.Status == "Created")
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync(); // Auto-save changes
            return payment;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 