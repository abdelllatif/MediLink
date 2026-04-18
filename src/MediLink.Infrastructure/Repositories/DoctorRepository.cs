namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Data;

/// <summary>
/// Doctor-specific repository
/// </summary>
public interface IDoctorRepository : IRepository<Doctor>
{
    Task<Doctor?> GetDoctorByEmailAsync(string email);
    Task<IEnumerable<Doctor>> GetVerifiedDoctorsAsync();
    Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
    Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
}

public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Doctor?> GetDoctorByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Email == email && !d.IsDeleted);
    }

    public async Task<IEnumerable<Doctor>> GetVerifiedDoctorsAsync()
    {
        return await _dbSet
            .Where(d => !d.IsDeleted && d.IsVerified && d.IsActive)
            .OrderBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
    {
        return await _dbSet
            .Where(d => !d.IsDeleted && d.Specialization == specialization && d.IsVerified)
            .OrderBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
    {
        return await _dbSet
            .Include(d => d.TimeSlots)
            .Where(d => !d.IsDeleted && d.IsActive && d.IsVerified &&
                       d.TimeSlots.Any(ts => ts.Status == Domain.Enums.TimeSlotStatus.Available))
            .ToListAsync();
    }
}
