namespace MediLink.Application.Services;

using AutoMapper;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Repositories;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IRepository<Cabinet> _cabinetRepository;
    private readonly IMapper _mapper;

    public PatientService(
        IPatientRepository patientRepository, 
        IRepository<Cabinet> cabinetRepository,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _cabinetRepository = cabinetRepository;
        _mapper = mapper;
    }

    public async Task<PatientDto?> GetPatientAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient == null ? null : _mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }

    public async Task<PatientDto> CreatePatientAsync(CreatePatientDto dto)
    {
        var patient = _mapper.Map<Patient>(dto);
        patient.CreatedAt = DateTime.UtcNow;

        if (dto.CabinetId != Guid.Empty)
        {
            var cabinet = await _cabinetRepository.GetByIdAsync(dto.CabinetId);
            if (cabinet != null)
            {
                patient.Cabinets.Add(cabinet);
            }
        }

        await _patientRepository.AddAsync(patient);
        return _mapper.Map<PatientDto>(patient);
    }

    public async Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        _mapper.Map(dto, patient);
        await _patientRepository.UpdateAsync(patient);
        return _mapper.Map<PatientDto>(patient);
    }

    public async Task DeletePatientAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        await _patientRepository.DeleteAsync(patient);
    }

    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query)
    {
        var patients = await _patientRepository.SearchPatientAsync(query);
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}
