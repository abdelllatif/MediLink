namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Data;

/// <summary>
/// Appointment-specific repository
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(Guid doctorId);
    Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int daysAhead = 7);
    Task<int> GetPendingPaymentCountAsync();
}

public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(Guid patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Include(a => a.TimeSlot)
            .Where(a => !a.IsDeleted && a.PatientId == patientId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(Guid doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.TimeSlot)
            .Where(a => !a.IsDeleted && a.DoctorId == doctorId)
            .OrderBy(a => a.TimeSlot.Date)
            .ThenBy(a => a.TimeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => !a.IsDeleted && a.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int daysAhead = 7)
    {
        var futureDate = DateTime.UtcNow.AddDays(daysAhead);
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.TimeSlot)
            .Where(a => !a.IsDeleted && 
                       a.Status == AppointmentStatus.Scheduled &&
                       a.TimeSlot.Date <= futureDate)
            .OrderBy(a => a.TimeSlot.Date)
            .ToListAsync();
    }

    public async Task<int> GetPendingPaymentCountAsync()
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.Status == AppointmentStatus.PendingPayment)
            .CountAsync();
    }
}
