namespace MediLink.Application.Handlers;

using AutoMapper;
using MediatR;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Application.Requests;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, AppointmentDto>
{
    private readonly IAppointmentService _appointmentService;

    public BookAppointmentCommandHandler(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<AppointmentDto> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        return await _appointmentService.BookAppointmentAsync(request.Appointment);
    }
}
