namespace MediLink.Application.Requests;

using MediatR;
using MediLink.Application.DTOs;

public class CreateMedicalDocumentCommand : IRequest<MedicalDocumentDto>
{
    public CreateMedicalDocumentDto Document { get; set; } = null!;
}
