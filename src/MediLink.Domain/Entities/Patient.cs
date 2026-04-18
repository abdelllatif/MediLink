namespace MediLink.Domain.Entities;

using MediLink.Domain.ValueObjects;

/// <summary>
/// Patient entity - Represents a patient in the system
/// </summary>
public class Patient : User
{
    public string? CIN { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }

    // Navigation Properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public int GetAge() => DateTime.Now.Year - DateOfBirth.Year;
}
