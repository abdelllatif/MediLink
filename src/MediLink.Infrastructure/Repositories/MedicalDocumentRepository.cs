namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Data;

/// <summary>
/// MedicalDocument-specific repository
/// </summary>
public interface IMedicalDocumentRepository : IRepository<MedicalDocument>
{
    Task<IEnumerable<MedicalDocument>> GetPatientDocumentsAsync(Guid patientId);
    Task<IEnumerable<MedicalDocument>> GetDoctorDocumentsAsync(Guid doctorId);
    Task<IEnumerable<MedicalDocument>> GetArchivedDocumentsAsync(Guid patientId);
    Task<IEnumerable<MedicalDocument>> GetRecentDocumentsAsync(Guid patientId, int days = 30);
    Task<int> GetDocumentCountAsync(Guid patientId);
}

public class MedicalDocumentRepository : BaseRepository<MedicalDocument>, IMedicalDocumentRepository
{
    public MedicalDocumentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MedicalDocument>> GetPatientDocumentsAsync(Guid patientId)
    {
        return await _dbSet
            .Include(md => md.Doctor)
            .Where(md => !md.IsDeleted && md.PatientId == patientId && !md.IsArchived)
            .OrderByDescending(md => md.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalDocument>> GetDoctorDocumentsAsync(Guid doctorId)
    {
        return await _dbSet
            .Include(md => md.Patient)
            .Where(md => !md.IsDeleted && md.DoctorId == doctorId)
            .OrderByDescending(md => md.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalDocument>> GetArchivedDocumentsAsync(Guid patientId)
    {
        return await _dbSet
            .Where(md => !md.IsDeleted && md.PatientId == patientId && md.IsArchived)
            .OrderByDescending(md => md.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalDocument>> GetRecentDocumentsAsync(Guid patientId, int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _dbSet
            .Where(md => !md.IsDeleted && 
                        md.PatientId == patientId &&
                        md.CreatedAt >= cutoffDate &&
                        !md.IsArchived)
            .OrderByDescending(md => md.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetDocumentCountAsync(Guid patientId)
    {
        return await _dbSet
            .Where(md => !md.IsDeleted && md.PatientId == patientId && !md.IsArchived)
            .CountAsync();
    }
}
