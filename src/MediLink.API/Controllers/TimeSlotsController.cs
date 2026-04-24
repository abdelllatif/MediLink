namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Generalist,Admin")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId, [FromQuery] DateTime date)
    {
        var timeSlots = await _timeSlotService.GetAvailableTimeSlotsAsync(doctorId, date);
        return Ok(timeSlots);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotDto dto)
    {
        var timeSlot = await _timeSlotService.CreateTimeSlotAsync(dto);
        return CreatedAtAction(nameof(GetByDoctor), new { doctorId = timeSlot.DoctorId, date = timeSlot.Date }, timeSlot);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateTimeSlotsDto dto)
    {
        await _timeSlotService.GenerateTimeSlotsAsync(dto);
        return NoContent();
    }

    [HttpPost("block-day")]
    public async Task<IActionResult> BlockDay([FromBody] BlockDayDto dto)
    {
        await _timeSlotService.BlockDayAsync(dto);
        return NoContent();
    }

    [HttpPost("unblock-day")]
    public async Task<IActionResult> UnblockDay([FromBody] UnblockDayDto dto)
    {
        await _timeSlotService.UnblockDayAsync(dto.DoctorId, dto.Date);
        return NoContent();
    }
}
