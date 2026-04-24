namespace MediLink.Domain.Entities;

using MediLink.Domain.Enums;

/// <summary>
/// Appointment entity - Represents a medical appointment/consultation
/// </summary>
public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TimeSlotId { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Symptoms { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; } = 150m; // DH

    // Vitals
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;

    public bool CanBeCancelled() => 
        Status is AppointmentStatus.Scheduled or AppointmentStatus.PendingPayment;

    public bool IsUpcoming() => TimeSlot.GetDateTime() > DateTime.Now;
}
