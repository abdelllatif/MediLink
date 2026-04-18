namespace MediLink.Domain.Entities;

/// <summary>
/// Specialist entity - Represents a specialist doctor
/// </summary>
public class Specialist : User
{
    public string Specialization { get; set; } = null!;
    public string? HospitalName { get; set; }
    public string? HospitalPhone { get; set; }
    public int ExperienceYears { get; set; }
    public decimal ConsultationPrice { get; set; } = 200m; // DH
    public string? LicenseNumber { get; set; }
    public bool IsVerified { get; set; }

    // Navigation Properties
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
}
