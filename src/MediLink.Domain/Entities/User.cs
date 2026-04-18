namespace MediLink.Domain.Entities;

using MediLink.Domain.Enums;

/// <summary>
/// Base User class (polymorphic - inherited by Patient, Doctor, Nurse, Admin)
/// </summary>
public abstract class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastLoginAt { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}";
}
