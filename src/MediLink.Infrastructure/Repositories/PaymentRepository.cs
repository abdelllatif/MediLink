namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Data;

/// <summary>
/// Payment-specific repository
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByStripeTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetPatientPaymentsAsync(Guid patientId);
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetPendingPaymentsCountAsync();
}

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByStripeTransactionIdAsync(string transactionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.StripeTransactionId == transactionId);
    }

    public async Task<IEnumerable<Payment>> GetPatientPaymentsAsync(Guid patientId)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.PatientId == patientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.Status == status)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    public async Task<int> GetPendingPaymentsCountAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Pending)
            .CountAsync();
    }
}
