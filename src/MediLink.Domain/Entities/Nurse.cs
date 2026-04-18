namespace MediLink.Domain.Entities;

/// <summary>
/// Nurse entity - Represents a nurse in the system
/// </summary>
public class Nurse : User
{
    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public string? Department { get; set; }
    public string? License { get; set; }
}
