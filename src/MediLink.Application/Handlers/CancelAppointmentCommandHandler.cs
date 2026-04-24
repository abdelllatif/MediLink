namespace MediLink.Application.Handlers;

using MediatR;
using MediLink.Application.Interfaces;
using MediLink.Application.Requests;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
{
    private readonly IAppointmentService _appointmentService;

    public CancelAppointmentCommandHandler(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        await _appointmentService.CancelAppointmentAsync(request.AppointmentId);
        return Unit.Value;
    }
}
