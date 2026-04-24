namespace MediLink.Application.Services;

using AutoMapper;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Repositories;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        ITimeSlotRepository timeSlotRepository,
        IPatientRepository patientRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _timeSlotRepository = timeSlotRepository;
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentDto> BookAppointmentAsync(CreateAppointmentDto dto)
    {
        var timeslot = await _timeSlotRepository.GetByIdAsync(dto.TimeSlotId);
        if (timeslot == null)
        {
            throw new KeyNotFoundException("Time slot not found");
        }

        if (timeslot.Status != TimeSlotStatus.Available)
        {
            throw new InvalidOperationException("Time slot is not available");
        }

        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = timeslot.DoctorId,
            TimeSlotId = dto.TimeSlotId,
            Status = AppointmentStatus.Scheduled,
            TotalAmount = timeslot.Price,
            CreatedAt = DateTime.UtcNow
        };

        await _appointmentRepository.BeginTransactionAsync();
        try
        {
            await _appointmentRepository.AddAsync(appointment);
            timeslot.Status = TimeSlotStatus.Reserved;
            timeslot.AppointmentId = appointment.Id;
            await _timeSlotRepository.UpdateAsync(timeslot);
            await _appointmentRepository.CommitTransactionAsync();
        }
        catch
        {
            await _appointmentRepository.RollbackTransactionAsync();
            throw;
        }

        return _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<AppointmentDto?> GetAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId)
    {
        var appointments = await _appointmentRepository.GetPatientAppointmentsAsync(patientId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentDto>> GetTodayAppointmentsAsync()
    {
        var appointments = await _appointmentRepository.GetTodayAppointmentsAsync();
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        if (!appointment.CanBeCancelled())
        {
            throw new InvalidOperationException("Appointment cannot be cancelled at this stage");
        }

        var timeslot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
        await _appointmentRepository.BeginTransactionAsync();
        try
        {
            appointment.Status = AppointmentStatus.Cancelled;
            await _appointmentRepository.UpdateAsync(appointment);

            if (timeslot != null)
            {
                timeslot.Status = TimeSlotStatus.Available;
                timeslot.AppointmentId = null;
                await _timeSlotRepository.UpdateAsync(timeslot);
            }

            await _appointmentRepository.CommitTransactionAsync();
        }
        catch
        {
            await _appointmentRepository.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task CompleteAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        appointment.Status = AppointmentStatus.Completed;
        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<AppointmentDto> UpdateVitalsAsync(Guid id, UpdateVitalsDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        appointment.BloodPressure = dto.BloodPressure;
        appointment.HeartRate = dto.HeartRate;
        appointment.Temperature = dto.Temperature;
        appointment.Weight = dto.Weight;

        await _appointmentRepository.UpdateAsync(appointment);
        return _mapper.Map<AppointmentDto>(appointment);
    }
}
