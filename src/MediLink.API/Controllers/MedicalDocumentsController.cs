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
[Authorize(Roles = "Nurse,Generalist,Specialist,Admin")]
public class MedicalDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMedicalDocumentService _documentService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public MedicalDocumentsController(
        IMediator mediator,
        IMedicalDocumentService documentService,
        IHubContext<NotificationHub> hubContext)
    {
        _mediator = mediator;
        _documentService = documentService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicalDocumentDto dto)
    {
        var document = await _mediator.Send(new CreateMedicalDocumentCommand { Document = dto });
        await _hubContext.Clients.All.SendAsync("MedicalDocumentCreated", document);
        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var document = await _documentService.GetDocumentAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        return Ok(document);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var documents = await _documentService.GetPatientDocumentsAsync(patientId);
        return Ok(documents);
    }

    [HttpPost("{id:guid}/generate-pdf")]
    public async Task<IActionResult> GeneratePdf(Guid id)
    {
        var bytes = await _documentService.GeneratePdfAsync(id);
        return File(bytes, "application/pdf", $"medical-document-{id}.pdf");
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        await _documentService.ArchiveDocumentAsync(id);
        await _hubContext.Clients.All.SendAsync("MedicalDocumentArchived", new { DocumentId = id });
        return NoContent();
    }
}
