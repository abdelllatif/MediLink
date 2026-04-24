namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Generalist,Admin")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _patientService.GetPatientAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var patients = await _patientService.SearchPatientsAsync(query);
        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        var patient = await _patientService.CreatePatientAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        var patient = await _patientService.UpdatePatientAsync(id, dto);
        return Ok(patient);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _patientService.DeletePatientAsync(id);
        return NoContent();
    }
}
