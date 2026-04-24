namespace MediLink.Application.DTOs;

/// <summary>
/// DTO for TimeSlot entity.
/// </summary>
public class TimeSlotDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = string.Empty; // LIBRE, RÉSERVÉ, BLOQUÉ
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for creating time slots.
/// </summary>
public class CreateTimeSlotDto
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for generating time slots in a date range.
/// </summary>
public class GenerateTimeSlotsDto
{
    public Guid DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for blocking a full doctor day.
/// </summary>
public class BlockDayDto
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// DTO for unblocking a doctor day.
/// </summary>
public class UnblockDayDto
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
}
