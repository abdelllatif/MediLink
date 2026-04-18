namespace MediLink.Shared.Constants;

/// <summary>
/// Application-wide constants.
/// </summary>
public static class AppConstants
{
    /// <summary>Date format for API responses</summary>
    public const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    /// <summary>Time zone for the application</summary>
    public const string TimeZone = "Morocco Standard Time";

    /// <summary>Working days of the week (0 = Sunday, 6 = Saturday)</summary>
    public static readonly int[] WorkingDays = { 1, 2, 3, 4, 5 }; // Monday to Friday

    /// <summary>Time slot duration in minutes</summary>
    public const int TimeSlotDurationMinutes = 30;

    /// <summary>API start hour</summary>
    public const int TimeSlotStartHour = 9;

    /// <summary>API end hour</summary>
    public const int TimeSlotEndHour = 17;

    /// <summary>Maximum days to display in advance</summary>
    public const int MaxDaysAhead = 60;

    /// <summary>Document retention period in days (7 years)</summary>
    public const int DocumentRetentionDays = 2555;

    /// <summary>JWT expiry in minutes</summary>
    public const int JwtExpiryMinutes = 60;

    /// <summary>JWT refresh token expiry in days</summary>
    public const int JwtRefreshTokenExpiryDays = 7;
}

/// <summary>
/// Role-related constants.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Generalist = "Generalist";
    public const string Specialist = "Specialist";
    public const string Nurse = "Nurse";
    public const string Patient = "Patient";
}

/// <summary>
/// Pricing constants.
/// </summary>
public static class Prices
{
    /// <summary>Basic consultation price in DH</summary>
    public const decimal ConsultationPrice = 150m;

    /// <summary>Appointment advance payment in DH</summary>
    public const decimal AppointmentAdvancePayment = 50m;

    /// <summary>Default specialist consultation price in DH</summary>
    public const decimal DefaultSpecialistPrice = 200m;
}

/// <summary>
/// Message templates.
/// </summary>
public static class MessageTemplates
{
    public const string SuccessMessage = "Operation completed successfully.";
    public const string ErrorMessage = "An error occurred during the operation.";
    public const string UnauthorizedMessage = "You are not authorized to perform this action.";
    public const string NotFoundMessage = "The requested resource was not found.";
    public const string ConflictMessage = "The operation conflicts with existing data.";
}
