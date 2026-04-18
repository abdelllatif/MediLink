namespace MediLink.Domain.Entities;

/// <summary>
/// BlockedDay entity - Represents a day when a doctor is unavailable
/// </summary>
public class BlockedDay : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; } = null!;
}
