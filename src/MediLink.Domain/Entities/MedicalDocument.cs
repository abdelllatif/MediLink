namespace MediLink.Domain.Entities;

/// <summary>
/// MedicalDocument entity - Represents a medical report/prescription after consultation
/// </summary>
public class MedicalDocument : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? SpecialistId { get; set; }
    public Guid? AppointmentId { get; set; }

    public string Diagnosis { get; set; } = null!;
    public string Prescription { get; set; } = null!;
    public string? Notes { get; set; }
    public string? PdfUrl { get; set; }
    public decimal Total { get; set; }
    public bool IsArchived { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Specialist? Specialist { get; set; }
    public Appointment? Appointment { get; set; }
    public ICollection<MedicalAct> MedicalActs { get; set; } = new List<MedicalAct>();
}
