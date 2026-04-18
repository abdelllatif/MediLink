namespace MediLink.Application.DTOs;

/// <summary>
/// DTO for MedicalDocument entity.
/// </summary>
public class MedicalDocumentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? SpecialistId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating a medical document.
/// </summary>
public class CreateMedicalDocumentDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? SpecialistId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}
