namespace MediLink.Domain.Entities;

using MediLink.Domain.ValueObjects;

/// <summary>
/// Patient entity - Represents a patient in the system
/// </summary>
public class Patient : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CIN { get; set; }
    public Address? Address { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }

    // Navigation Properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
    public ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();

    public int GetAge() => DateTime.Now.Year - DateOfBirth.Year;
    public string GetFullName() => $"{FirstName} {LastName}";
}
