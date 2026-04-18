namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Data;

/// <summary>
/// Patient-specific repository
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetPatientByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchPatientAsync(string query);
    Task<IEnumerable<Patient>> GetPatientsWithAppointmentsAsync();
}

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetPatientByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Email == email && !p.IsDeleted);
    }

    public async Task<IEnumerable<Patient>> SearchPatientAsync(string query)
    {
        var lowerQuery = query.ToLower();
        return await _dbSet
            .Where(p => !p.IsDeleted && (
                p.FirstName.ToLower().Contains(lowerQuery) ||
                p.LastName.ToLower().Contains(lowerQuery) ||
                p.Email.ToLower().Contains(lowerQuery) ||
                (p.CIN != null && p.CIN.Contains(query)) ||
                (p.PhoneNumber != null && p.PhoneNumber.Contains(query))
            ))
            .OrderBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetPatientsWithAppointmentsAsync()
    {
        return await _dbSet
            .Include(p => p.Appointments)
            .Where(p => !p.IsDeleted && p.Appointments.Count > 0)
            .ToListAsync();
    }
}
