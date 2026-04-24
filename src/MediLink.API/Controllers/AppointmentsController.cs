namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using MediLink.API.Hubs;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Application.Requests;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Generalist,Admin")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppointmentService _appointmentService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AppointmentsController(
        IMediator mediator,
        IAppointmentService appointmentService,
        IHubContext<NotificationHub> hubContext)
    {
        _mediator = mediator;
        _appointmentService = appointmentService;
        _hubContext = hubContext;
    }

    [HttpPost("book")]
    public async Task<IActionResult> Book([FromBody] CreateAppointmentDto dto)
    {
        var appointment = await _mediator.Send(new BookAppointmentCommand { Appointment = dto });
        await _hubContext.Clients.All.SendAsync("AppointmentBooked", appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetAppointmentAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        return Ok(appointment);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
        return Ok(appointments);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var appointments = await _appointmentService.GetTodayAppointmentsAsync();
        return Ok(appointments);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _mediator.Send(new CancelAppointmentCommand { AppointmentId = id });
        await _hubContext.Clients.All.SendAsync("AppointmentCancelled", new { AppointmentId = id });
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await _mediator.Send(new CompleteAppointmentCommand { AppointmentId = id });
        await _hubContext.Clients.All.SendAsync("AppointmentCompleted", new { AppointmentId = id });
        return NoContent();
    }

    [HttpPut("{id:guid}/vitals")]
    public async Task<IActionResult> UpdateVitals(Guid id, [FromBody] UpdateVitalsDto dto)
    {
        var appointment = await _appointmentService.UpdateVitalsAsync(id, dto);
        await _hubContext.Clients.All.SendAsync("PatientVitalsUpdated", appointment);
        return Ok(appointment);
    }
}
