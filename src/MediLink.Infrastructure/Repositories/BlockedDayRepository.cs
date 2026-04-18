namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Data;

/// <summary>
/// BlockedDay-specific repository
/// </summary>
public interface IBlockedDayRepository : IRepository<BlockedDay>
{
    Task<IEnumerable<BlockedDay>> GetDoctorBlockedDaysAsync(Guid doctorId);
    Task<IEnumerable<BlockedDay>> GetBlockedDaysInRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    Task<bool> IsDayBlockedAsync(Guid doctorId, DateTime date);
}

public class BlockedDayRepository : BaseRepository<BlockedDay>, IBlockedDayRepository
{
    public BlockedDayRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BlockedDay>> GetDoctorBlockedDaysAsync(Guid doctorId)
    {
        return await _dbSet
            .Where(bd => !bd.IsDeleted && bd.DoctorId == doctorId)
            .OrderBy(bd => bd.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlockedDay>> GetBlockedDaysInRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(bd => !bd.IsDeleted &&
                        bd.DoctorId == doctorId &&
                        bd.Date >= startDate &&
                        bd.Date <= endDate)
            .OrderBy(bd => bd.Date)
            .ToListAsync();
    }

    public async Task<bool> IsDayBlockedAsync(Guid doctorId, DateTime date)
    {
        return await _dbSet
            .AnyAsync(bd => !bd.IsDeleted &&
                           bd.DoctorId == doctorId &&
                           bd.Date == date.Date);
    }
}
