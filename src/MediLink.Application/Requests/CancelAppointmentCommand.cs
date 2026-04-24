namespace MediLink.Application.Requests;

using MediatR;

public class CancelAppointmentCommand : IRequest
{
    public Guid AppointmentId { get; set; }
}
