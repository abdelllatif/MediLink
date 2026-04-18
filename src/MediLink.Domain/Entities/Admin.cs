namespace MediLink.Domain.Entities;

/// <summary>
/// Admin entity - Represents an administrator in the system
/// </summary>
public class Admin : User
{
    public string? Department { get; set; }
    public DateTime PromotedAt { get; set; }
}
