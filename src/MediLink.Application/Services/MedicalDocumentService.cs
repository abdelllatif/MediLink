namespace MediLink.Application.Services;

using System.IO;
using AutoMapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Repositories;

public class MedicalDocumentService : IMedicalDocumentService
{
    private readonly IMedicalDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public MedicalDocumentService(
        IMedicalDocumentRepository documentRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<MedicalDocumentDto> CreateDocumentAsync(CreateMedicalDocumentDto dto)
    {
        var document = _mapper.Map<MedicalDocument>(dto);
        document.Total = 0m;
        document.IsArchived = false;
        document.CreatedAt = DateTime.UtcNow;

        await _documentRepository.AddAsync(document);
        var result = _mapper.Map<MedicalDocumentDto>(document);
        return result;
    }

    public async Task<MedicalDocumentDto?> GetDocumentAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        return document == null ? null : _mapper.Map<MedicalDocumentDto>(document);
    }

    public async Task<IEnumerable<MedicalDocumentDto>> GetPatientDocumentsAsync(Guid patientId)
    {
        var documents = await _documentRepository.GetPatientDocumentsAsync(patientId);
        return _mapper.Map<IEnumerable<MedicalDocumentDto>>(documents);
    }

    public async Task<byte[]> GeneratePdfAsync(Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            throw new KeyNotFoundException("Medical document not found");
        }

        using var stream = new MemoryStream();
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.Header().Text("MediLink Medical Report").SemiBold().FontSize(20).FontColor(Colors.Black);
                page.Content().Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Text($"Document ID: {document.Id}").FontSize(12);
                    column.Item().Text($"Patient ID: {document.PatientId}").FontSize(12);
                    column.Item().Text($"Doctor ID: {document.DoctorId}").FontSize(12);
                    if (document.SpecialistId != null)
                    {
                        column.Item().Text($"Specialist ID: {document.SpecialistId}").FontSize(12);
                    }

                    column.Item().Text("Diagnosis").Bold().FontSize(14);
                    column.Item().Text(document.Diagnosis).FontSize(12);
                    column.Item().Text("Prescription").Bold().FontSize(14);
                    column.Item().Text(document.Prescription).FontSize(12);
                    if (!string.IsNullOrWhiteSpace(document.Notes))
                    {
                        column.Item().Text("Notes").Bold().FontSize(14);
                        column.Item().Text(document.Notes).FontSize(12);
                    }
                    column.Item().Text($"Total: {document.Total:C}").FontSize(12);
                });
                page.Footer().AlignCenter().Text($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm}");
            });
        });

        pdf.GeneratePdf(stream);
        return stream.ToArray();
    }

    public async Task ArchiveDocumentAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            throw new KeyNotFoundException("Medical document not found");
        }

        document.IsArchived = true;
        await _documentRepository.UpdateAsync(document);
    }
}
