namespace MediLink.Application.DTOs;

/// <summary>
/// DTO for Appointment entity.
/// </summary>
public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TimeSlotId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    // Vitals
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
}

/// <summary>
/// DTO for creating/booking an appointment.
/// </summary>
public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid TimeSlotId { get; set; }
}

/// <summary>
/// DTO for appointment status update.
/// </summary>
public class UpdateAppointmentStatusDto
{
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO for recording vitals.
/// </summary>
public class UpdateVitalsDto
{
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
}
