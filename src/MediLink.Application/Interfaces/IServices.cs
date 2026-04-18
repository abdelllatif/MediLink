namespace MediLink.Application.Interfaces;

using MediLink.Application.DTOs;

/// <summary>
/// Interface for patient service operations.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    Task<PatientDto?> GetPatientAsync(Guid id);

    /// <summary>
    /// Gets all patients.
    /// </summary>
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync();

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    Task<PatientDto> CreatePatientAsync(CreatePatientDto dto);

    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto dto);

    /// <summary>
    /// Deletes a patient.
    /// </summary>
    Task DeletePatientAsync(Guid id);

    /// <summary>
    /// Searches for patients by name or phone.
    /// </summary>
    Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query);
}

/// <summary>
/// Interface for doctor service operations.
/// </summary>
public interface IDoctorService
{
    /// <summary>
    /// Gets a doctor by ID.
    /// </summary>
    Task<DoctorDto?> GetDoctorAsync(Guid id);

    /// <summary>
    /// Gets all doctors.
    /// </summary>
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();

    /// <summary>
    /// Gets available time slots for a doctor on a specific date.
    /// </summary>
    Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);

    /// <summary>
    /// Blocks an entire day for a doctor.
    /// </summary>
    Task BlockDayAsync(Guid doctorId, DateTime date, string? reason = null);

    /// <summary>
    /// Unblocks a day for a doctor.
    /// </summary>
    Task UnblockDayAsync(Guid doctorId, DateTime date);
}

/// <summary>
/// Interface for appointment service operations.
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Books a new appointment.
    /// </summary>
    Task<AppointmentDto> BookAppointmentAsync(CreateAppointmentDto dto);

    /// <summary>
    /// Gets an appointment by ID.
    /// </summary>
    Task<AppointmentDto?> GetAppointmentAsync(Guid id);

    /// <summary>
    /// Gets all appointments for a patient.
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId);

    /// <summary>
    /// Cancels an appointment.
    /// </summary>
    Task CancelAppointmentAsync(Guid id);

    /// <summary>
    /// Completes an appointment.
    /// </summary>
    Task CompleteAppointmentAsync(Guid id);
}

/// <summary>
/// Interface for medical document service operations.
/// </summary>
public interface IMedicalDocumentService
{
    /// <summary>
    /// Creates a new medical document.
    /// </summary>
    Task<MedicalDocumentDto> CreateDocumentAsync(CreateMedicalDocumentDto dto);

    /// <summary>
    /// Gets a medical document by ID.
    /// </summary>
    Task<MedicalDocumentDto?> GetDocumentAsync(Guid id);

    /// <summary>
    /// Gets all medical documents for a patient.
    /// </summary>
    Task<IEnumerable<MedicalDocumentDto>> GetPatientDocumentsAsync(Guid patientId);

    /// <summary>
    /// Generates a PDF for a medical document.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(Guid documentId);

    /// <summary>
    /// Archives a medical document.
    /// </summary>
    Task ArchiveDocumentAsync(Guid id);
}

/// <summary>
/// Interface for payment service operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Creates a Stripe checkout session.
    /// </summary>
    Task<string> CreateCheckoutSessionAsync(Guid appointmentId);

    /// <summary>
    /// Gets a payment by ID.
    /// </summary>
    Task<PaymentDto?> GetPaymentAsync(Guid id);

    /// <summary>
    /// Processes a Stripe webhook event.
    /// </summary>
    Task ProcessWebhookAsync(string json, string signature);

    /// <summary>
    /// Gets all payments for an appointment.
    /// </summary>
    Task<IEnumerable<PaymentDto>> GetAppointmentPaymentsAsync(Guid appointmentId);
}
