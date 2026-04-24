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
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ReverseMap();

        CreateMap<CreatePatientDto, Patient>();

        CreateMap<UpdatePatientDto, Patient>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.IsAvailable, opt => opt.Ignore());

        CreateMap<CreateDoctorDto, Doctor>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => Domain.Enums.UserRole.Generalist));

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.TotalAmount))
            .ReverseMap();

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.AppointmentStatus.Scheduled))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

        CreateMap<TimeSlot, TimeSlotDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();

        CreateMap<CreateTimeSlotDto, TimeSlot>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.TimeSlotStatus.Available));

        CreateMap<MedicalDocument, MedicalDocumentDto>()
            .ReverseMap();

        CreateMap<CreateMedicalDocumentDto, MedicalDocument>()
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(_ => false));

    }
}
