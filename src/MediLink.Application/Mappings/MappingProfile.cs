using MediLink.Application.DTOs;
using MediLink.Domain.Entities;

namespace MediLink.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between DTOs and domain entities.
/// </summary>
public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        // Patient Mappings
        CreateMap<Patient, PatientDto>()
            .ReverseMap();

        // Doctor Mappings
        CreateMap<Doctor, DoctorDto>()
            .ReverseMap();

        // Appointment Mappings
        CreateMap<Appointment, AppointmentDto>()
            .ReverseMap();

        // TimeSlot Mappings
        CreateMap<TimeSlot, TimeSlotDto>()
            .ReverseMap();

        // MedicalDocument Mappings
        CreateMap<MedicalDocument, MedicalDocumentDto>()
            .ReverseMap();
    }
}
