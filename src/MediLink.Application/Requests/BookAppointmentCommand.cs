namespace MediLink.Application.Requests;

using MediatR;
using MediLink.Application.DTOs;

public class BookAppointmentCommand : IRequest<AppointmentDto>
{
    public CreateAppointmentDto Appointment { get; set; } = null!;
}
