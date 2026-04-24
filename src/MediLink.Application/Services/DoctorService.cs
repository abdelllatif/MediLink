namespace MediLink.Application.Services;

using AutoMapper;
using BCrypt.Net;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Repositories;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ITimeSlotService _timeSlotService;
    private readonly IMapper _mapper;

    public DoctorService(
        IDoctorRepository doctorRepository,
        ITimeSlotService timeSlotService,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _timeSlotService = timeSlotService;
        _mapper = mapper;
    }

    public async Task<DoctorDto?> GetDoctorAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null)
        {
            return null;
        }

        var dto = _mapper.Map<DoctorDto>(doctor);
        var todaySlots = await _timeSlotService.GetAvailableTimeSlotsAsync(id, DateTime.UtcNow.Date);
        dto.IsAvailable = todaySlots.Any();
        return dto;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        return result;
    }

    public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto)
    {
        var doctor = _mapper.Map<Doctor>(dto);
        doctor.PasswordHash = BCrypt.HashPassword(dto.Password);
        doctor.Role = Domain.Enums.UserRole.Generalist;
        doctor.IsVerified = false;
        doctor.IsActive = true;
        doctor.CreatedAt = DateTime.UtcNow;

        await _doctorRepository.AddAsync(doctor);
        var result = _mapper.Map<DoctorDto>(doctor);
        result.IsAvailable = false;
        return result;
    }

    public Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date)
    {
        return _timeSlotService.GetAvailableTimeSlotsAsync(doctorId, date);
    }

    public Task BlockDayAsync(Guid doctorId, DateTime date, string? reason = null)
    {
        return _timeSlotService.BlockDayAsync(new BlockDayDto
        {
            DoctorId = doctorId,
            Date = date,
            Reason = reason
        });
    }

    public Task UnblockDayAsync(Guid doctorId, DateTime date)
    {
        return _timeSlotService.UnblockDayAsync(doctorId, date);
    }
}
