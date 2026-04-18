namespace MediLink.Domain.Entities;

using MediLink.Domain.Enums;

/// <summary>
/// MedicalAct entity - Represents a medical act/procedure (IRM, Radiography, ECG, etc.)
/// </summary>
public class MedicalAct : BaseEntity
{
    public Guid MedicalDocumentId { get; set; }
    public MedicalActType Type { get; set; }
    public string Description { get; set; } = null!;
    public decimal Cost { get; set; }
    public string? Result { get; set; }
    public DateTime? PerformedAt { get; set; }

    // Navigation Properties
    public MedicalDocument MedicalDocument { get; set; } = null!;
}
