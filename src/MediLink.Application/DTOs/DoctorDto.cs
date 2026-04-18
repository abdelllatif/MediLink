namespace MediLink.Application.DTOs;

/// <summary>
/// DTO for Doctor entity.
/// </summary>
public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Cabinet { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// DTO for creating a new doctor.
/// </summary>
public class CreateDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Cabinet { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}
