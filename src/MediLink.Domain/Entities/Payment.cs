namespace MediLink.Domain.Entities;

using MediLink.Domain.Enums;

/// <summary>
/// Payment entity - Represents a payment transaction (Stripe)
/// </summary>
public class Payment : BaseEntity
{
    public Guid? AppointmentId { get; set; }
    public Guid? MedicalDocumentId { get; set; }
    public Guid PatientId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD"; // Moroccan Dirham
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? StripeTransactionId { get; set; }
    public string? StripeSessionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }

    // Navigation Properties
    public Appointment? Appointment { get; set; }
    public MedicalDocument? MedicalDocument { get; set; }
    public Patient Patient { get; set; } = null!;

    public bool IsSuccessful() => Status == PaymentStatus.Completed;
    public bool CanBeRetried() => Status is PaymentStatus.Pending or PaymentStatus.Failed;
}
