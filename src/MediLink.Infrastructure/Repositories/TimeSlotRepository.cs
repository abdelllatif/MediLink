namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Data;

/// <summary>
/// TimeSlot-specific repository
/// </summary>
public interface ITimeSlotRepository : IRepository<TimeSlot>
{
    Task<IEnumerable<TimeSlot>> GetDoctorTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);
    Task<IEnumerable<TimeSlot>> GetBlockedTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    Task<int> GetAvailableSlotsCountAsync(Guid doctorId);
    Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime date, TimeSpan time);
}

public class TimeSlotRepository : BaseRepository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimeSlot>> GetDoctorTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(ts => !ts.IsDeleted && 
                        ts.DoctorId == doctorId &&
                        ts.Date >= startDate &&
                        ts.Date <= endDate)
            .OrderBy(ts => ts.Date)
            .ThenBy(ts => ts.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _dbSet
            .Where(ts => !ts.IsDeleted &&
                        ts.DoctorId == doctorId &&
                        ts.Date >= dayStart &&
                        ts.Date < dayEnd &&
                        ts.Status == TimeSlotStatus.Available)
            .OrderBy(ts => ts.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeSlot>> GetBlockedTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(ts => !ts.IsDeleted &&
                        ts.DoctorId == doctorId &&
                        ts.Date >= startDate &&
                        ts.Date <= endDate &&
                        ts.Status == TimeSlotStatus.Blocked)
            .ToListAsync();
    }

    public async Task<int> GetAvailableSlotsCountAsync(Guid doctorId)
    {
        return await _dbSet
            .Where(ts => !ts.IsDeleted &&
                        ts.DoctorId == doctorId &&
                        ts.Status == TimeSlotStatus.Available)
            .CountAsync();
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime date, TimeSpan time)
    {
        var slot = await _dbSet
            .FirstOrDefaultAsync(ts => !ts.IsDeleted &&
                                      ts.DoctorId == doctorId &&
                                      ts.Date == date.Date &&
                                      ts.StartTime == time &&
                                      ts.Status == TimeSlotStatus.Available);

        return slot != null;
    }
}
