namespace MediLink.Application.Requests;

using MediatR;

public class CompleteAppointmentCommand : IRequest
{
    public Guid AppointmentId { get; set; }
}
