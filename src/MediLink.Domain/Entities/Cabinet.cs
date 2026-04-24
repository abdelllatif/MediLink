namespace MediLink.Domain.Entities;

/// <summary>
/// Cabinet entity - Represents a clinic or medical cabinet
/// </summary>
public class Cabinet : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }

    // Navigation Properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
