namespace MediLink.Domain.Enums;

/// <summary>
/// Enumeration for user roles in the system.
/// </summary>
public enum UserRole
{
    /// <summary>System administrator</summary>
    Admin = 0,

    /// <summary>General practitioner (médecin généraliste)</summary>
    Generalist = 1,

    /// <summary>Specialist doctor (médecin spécialiste)</summary>
    Specialist = 2,

    /// <summary>Nurse (infirmier)</summary>
    Nurse = 3
}

/// <summary>
/// Enumeration for appointment statuses.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>Appointment is scheduled</summary>
    Scheduled = 0,

    /// <summary>Appointment is in progress</summary>
    InProgress = 1,

    /// <summary>Appointment is completed</summary>
    Completed = 2,

    /// <summary>Appointment was cancelled</summary>
    Cancelled = 3,

    /// <summary>Payment is pending</summary>
    PendingPayment = 4,

    /// <summary>Patient did not show up</summary>
    NoShow = 5
}

/// <summary>
/// Enumeration for time slot statuses.
/// </summary>
public enum TimeSlotStatus
{
    /// <summary>Slot is available (LIBRE)</summary>
    Available = 0,

    /// <summary>Slot is reserved (RÉSERVÉ)</summary>
    Reserved = 1,

    /// <summary>Slot is blocked (BLOQUÉ)</summary>
    Blocked = 2
}

/// <summary>
/// Enumeration for payment statuses.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Payment is pending</summary>
    Pending = 0,

    /// <summary>Payment is being processed</summary>
    Processing = 1,

    /// <summary>Payment is completed</summary>
    Completed = 2,

    /// <summary>Payment failed</summary>
    Failed = 3,

    /// <summary>Payment was refunded</summary>
    Refunded = 4
}

/// <summary>
/// Enumeration for medical act types.
/// </summary>
public enum MedicalActType
{
    /// <summary>MRI scan</summary>
    IRM = 0,

    /// <summary>X-ray radiography</summary>
    Radiography = 1,

    /// <summary>Electrocardiogram</summary>
    ECG = 2,

    /// <summary>Blood test</summary>
    BloodTest = 3,

    /// <summary>Urine test</summary>
    UrineTest = 4,

    /// <summary>Ultrasound</summary>
    Ultrasound = 5,

    /// <summary>CT scan</summary>
    CTScan = 6
}

/// <summary>
/// Enumeration for priority levels in specialist requests.
/// </summary>
public enum PriorityLevel
{
    /// <summary>Urgent priority (URGENT)</summary>
    Urgent = 0,

    /// <summary>Normal priority (NORMAL)</summary>
    Normal = 1,

    /// <summary>Non-urgent priority (NON-URGENT)</summary>
    NonUrgent = 2
}
