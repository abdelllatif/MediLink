namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Generalist,Admin")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        return Ok(doctors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doctor = await _doctorService.GetDoctorAsync(id);
        if (doctor == null)
        {
            return NotFound();
        }

        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
    {
        var doctor = await _doctorService.CreateDoctorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
    }

    [HttpGet("{id:guid}/available-time-slots")]
    public async Task<IActionResult> GetAvailableTimeSlots(Guid id, [FromQuery] DateTime date)
    {
        var timeSlots = await _doctorService.GetAvailableTimeSlotsAsync(id, date);
        return Ok(timeSlots);
    }

    [HttpPost("{id:guid}/block-day")]
    public async Task<IActionResult> BlockDay(Guid id, [FromBody] BlockDayDto dto)
    {
        await _doctorService.BlockDayAsync(id, dto.Date, dto.Reason);
        return NoContent();
    }

    [HttpPost("{id:guid}/unblock-day")]
    public async Task<IActionResult> UnblockDay(Guid id, [FromBody] UnblockDayDto dto)
    {
        await _doctorService.UnblockDayAsync(id, dto.Date);
        return NoContent();
    }
}
