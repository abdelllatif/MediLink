namespace MediLink.Application.Handlers;

using MediatR;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Application.Requests;

public class CreateMedicalDocumentCommandHandler : IRequestHandler<CreateMedicalDocumentCommand, MedicalDocumentDto>
{
    private readonly IMedicalDocumentService _documentService;

    public CreateMedicalDocumentCommandHandler(IMedicalDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<MedicalDocumentDto> Handle(CreateMedicalDocumentCommand request, CancellationToken cancellationToken)
    {
        return await _documentService.CreateDocumentAsync(request.Document);
    }
}
