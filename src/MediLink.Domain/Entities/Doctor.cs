namespace MediLink.Domain.Entities;

/// <summary>
/// Doctor entity - Represents a generalist doctor in the system
/// </summary>
public class Doctor : User
{
    public string Specialization { get; set; } = null!;
    public Guid CabinetId { get; set; }
    public int ExperienceYears { get; set; }
    public int MaxPatientsPerDay { get; set; } = 8;
    public string? LicenseNumber { get; set; }
    public bool IsVerified { get; set; }
    public decimal ConsultationPrice { get; set; } = 150m; // DH

    // Navigation Properties
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<BlockedDay> BlockedDays { get; set; } = new List<BlockedDay>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
    public Cabinet Cabinet { get; set; } = null!;
}
