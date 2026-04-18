namespace MediLink.Application.Services;

using AutoMapper;
using MediLink.Application.DTOs;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Repositories;

/// <summary>
/// TimeSlot service implementation
/// </summary>
public interface ITimeSlotService
{
    Task<TimeSlotDto?> GetTimeSlotAsync(Guid id);
    Task<IEnumerable<TimeSlotDto>> GetDoctorTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);
    Task<TimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto dto);
    Task GenerateTimeSlotsAsync(GenerateTimeSlotsDto dto);
    Task BlockDayAsync(BlockDayDto dto);
    Task UnblockDayAsync(Guid doctorId, DateTime date);
}

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _repository;
    private readonly IBlockedDayRepository _blockedDayRepository;
    private readonly IMapper _mapper;

    public TimeSlotService(
        ITimeSlotRepository repository,
        IBlockedDayRepository blockedDayRepository,
        IMapper mapper)
    {
        _repository = repository;
        _blockedDayRepository = blockedDayRepository;
        _mapper = mapper;
    }

    public async Task<TimeSlotDto?> GetTimeSlotAsync(Guid id)
    {
        var timeSlot = await _repository.GetByIdAsync(id);
        return timeSlot is null ? null : _mapper.Map<TimeSlotDto>(timeSlot);
    }

    public async Task<IEnumerable<TimeSlotDto>> GetDoctorTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        var timeSlots = await _repository.GetDoctorTimeSlotsAsync(doctorId, startDate, endDate);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots);
    }

    public async Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date)
    {
        var timeSlots = await _repository.GetAvailableTimeSlotsAsync(doctorId, date);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots);
    }

    public async Task<TimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto dto)
    {
        var timeSlot = _mapper.Map<TimeSlot>(dto);
        await _repository.AddAsync(timeSlot);
        return _mapper.Map<TimeSlotDto>(timeSlot);
    }

    public async Task GenerateTimeSlotsAsync(GenerateTimeSlotsDto dto)
    {
        var timeSlots = new List<TimeSlot>();
        var currentDate = dto.StartDate.Date;
        var endDate = dto.EndDate.Date;

        while (currentDate <= endDate)
        {
            // Skip weekends
            if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                // Check if day is blocked
                var isBlocked = await _blockedDayRepository.IsDayBlockedAsync(dto.DoctorId, currentDate);
                if (!isBlocked)
                {
                    // Create slots from 09:00 to 17:30 (30-minute intervals)
                    var currentTime = new TimeSpan(9, 0, 0);
                    var endTime = new TimeSpan(17, 30, 0);

                    while (currentTime < endTime)
                    {
                        var timeSlot = new TimeSlot
                        {
                            DoctorId = dto.DoctorId,
                            Date = currentDate,
                            StartTime = currentTime,
                            EndTime = currentTime.Add(TimeSpan.FromMinutes(30)),
                            Status = TimeSlotStatus.Available,
                            Price = dto.Price
                        };

                        timeSlots.Add(timeSlot);
                        currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        await _repository.AddRangeAsync(timeSlots);
    }

    public async Task BlockDayAsync(BlockDayDto dto)
    {
        // Create blocked day record
        var blockedDay = new BlockedDay
        {
            DoctorId = dto.DoctorId,
            Date = dto.Date.Date,
            Reason = dto.Reason
        };

        await _blockedDayRepository.AddAsync(blockedDay);

        // Update all time slots for this day to blocked
        var timeSlots = await _repository.GetDoctorTimeSlotsAsync(
            dto.DoctorId,
            dto.Date.Date,
            dto.Date.Date.AddDays(1));

        foreach (var timeSlot in timeSlots.Where(ts => ts.Status == TimeSlotStatus.Available))
        {
            timeSlot.Status = TimeSlotStatus.Blocked;
        }

        await _repository.UpdateRangeAsync(timeSlots);
    }

    public async Task UnblockDayAsync(Guid doctorId, DateTime date)
    {
        // Remove blocked day record
        var blockedDays = await _blockedDayRepository.GetBlockedDaysInRangeAsync(
            doctorId, date.Date, date.Date);

        foreach (var blockedDay in blockedDays)
        {
            await _blockedDayRepository.DeleteAsync(blockedDay);
        }

        // Update time slots back to available (if not reserved)
        var timeSlots = await _repository.GetDoctorTimeSlotsAsync(
            doctorId, date.Date, date.Date.AddDays(1));

        foreach (var timeSlot in timeSlots.Where(ts =>
            ts.Status == TimeSlotStatus.Blocked && ts.AppointmentId == null))
        {
            timeSlot.Status = TimeSlotStatus.Available;
        }

        await _repository.UpdateRangeAsync(timeSlots);
    }
}