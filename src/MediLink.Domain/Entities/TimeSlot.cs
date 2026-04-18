namespace MediLink.Domain.Entities;

using MediLink.Domain.Enums;

/// <summary>
/// TimeSlot entity - Represents a 30-minute consultation time slot (Créneau)
/// </summary>
public class TimeSlot : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSlotStatus Status { get; set; } = TimeSlotStatus.Available;
    public decimal Price { get; set; } = 150m; // DH
    public Guid? AppointmentId { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; } = null!;
    public Appointment? Appointment { get; set; }

    public DateTime GetDateTime() => Date.Add(StartTime);
    public bool IsExpired() => GetDateTime() < DateTime.Now;
}
