namespace MediLink.Application.Handlers;

using MediatR;
using MediLink.Application.Interfaces;
using MediLink.Application.Requests;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand>
{
    private readonly IAppointmentService _appointmentService;

    public CompleteAppointmentCommandHandler(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<Unit> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        await _appointmentService.CompleteAppointmentAsync(request.AppointmentId);
        return Unit.Value;
    }
}
