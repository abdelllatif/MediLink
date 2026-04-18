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
    public Guid? MedicalDocumentId { get; set; }
    public Guid? PaymentId { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Symptoms { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; } = 150m; // DH

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
    public MedicalDocument? MedicalDocument { get; set; }
    public Payment? Payment { get; set; }

    public bool CanBeCancelled() => 
        Status is AppointmentStatus.Scheduled or AppointmentStatus.PendingPayment;

    public bool IsUpcoming() => TimeSlot.GetDateTime() > DateTime.Now;
}
