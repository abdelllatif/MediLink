namespace MediLink.Application.DTOs;

/// <summary>
/// DTO for Payment entity.
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StripeTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating a payment.
/// </summary>
public class CreatePaymentDto
{
    public Guid? AppointmentId { get; set; }
    public Guid? MedicalDocumentId { get; set; }
    public decimal Amount { get; set; }
}
