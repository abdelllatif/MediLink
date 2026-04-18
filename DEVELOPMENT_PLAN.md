# 🚀 MediLink Development Plan - Complete Roadmap

**Date:** April 18, 2026  
**Project:** MediLink - Medical Care Management System  
**Status:** Planning Phase  

---

## 📊 DEVELOPMENT PHASES OVERVIEW

```
Phase 1: SETUP & PREREQUISITES (1-2 days)
    ↓
Phase 2: BACKEND CORE (Weeks 1-2)
    ├── Domain Layer
    ├── Infrastructure Layer
    └── Basic API
    ↓
Phase 3: BACKEND FEATURES (Weeks 2-3)
    ├── Authentication
    ├── Patient Management
    ├── Doctor Management
    └── Appointments
    ↓
Phase 4: ADVANCED FEATURES (Week 4)
    ├── Medical Documents
    ├── Payments (Stripe)
    └── Real-time (SignalR)
    ↓
Phase 5: FRONTEND SETUP (Week 5)
    ├── Project Setup
    ├── Base Components
    └── Authentication UI
    ↓
Phase 6: FRONTEND FEATURES (Weeks 5-6)
    ├── Dashboard
    ├── Booking System
    ├── Doctor Profiles
    └── Medical Documents
    ↓
Phase 7: INTEGRATION (Week 7)
    ├── API Integration
    ├── Real-time Updates
    └── Payment Flow
    ↓
Phase 8: TESTING & DEPLOYMENT (Week 8)
    ├── Unit Tests
    ├── Integration Tests
    └── Docker Deploy
```

---

## PHASE 1: SETUP & PREREQUISITES (1-2 days)

### Step 1.1: System Requirements Check

**Windows Installation:**

```powershell
# Check .NET version
dotnet --version
# Should be 8.0 or higher

# Check Node.js version
node --version
# Should be 18+ or higher

# Check npm version
npm --version
# Should be 9+

# Check Git
git --version
```

**Installation Order (if needed):**

1. **Visual Studio 2022** (or VS Code)
   - Download: https://visualstudio.microsoft.com/downloads/
   - Include: .NET development workload, ASP.NET Core
   - Installation time: ~30 min

2. **.NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Installation time: ~10 min

3. **SQL Server 2022 Developer Edition** (optional, can use LocalDB)
   - Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Or use LocalDB: Already included with Visual Studio
   - Installation time: ~20 min (if new install)

4. **Node.js 18 LTS**
   - Download: https://nodejs.org/
   - Installation time: ~5 min

5. **Git**
   - Download: https://git-scm.com/
   - Installation time: ~5 min

6. **Docker Desktop** (optional, for containerization)
   - Download: https://www.docker.com/products/docker-desktop
   - Installation time: ~15 min

7. **VS Code Extensions** (if using VS Code)
   - C# Dev Kit
   - REST Client
   - Prettier
   - ESLint
   - Tailwind CSS IntelliSense

### Step 1.2: Configure Development Environment

```bash
# 1. Navigate to project directory
cd C:\Users\Abdellatif Hissoune\MediLink

# 2. Restore .NET dependencies
dotnet restore

# 3. Check if everything is working
dotnet build

# 4. Navigate to frontend
cd frontend

# 5. Install Node dependencies
npm install

# 6. Verify frontend setup
npm run dev
# Press Ctrl+C to stop

cd ..
```

### Step 1.3: Database Setup

**Option A: Using LocalDB (Recommended for Development)**

```powershell
# Create database using SQL Server Object Explorer in Visual Studio
# Or use sqlcmd:

sqlcmd -S "(localdb)\mssqllocaldb" -Q "CREATE DATABASE MediLink"

# Verify
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT * FROM sys.databases WHERE name='MediLink'"
```

**Option B: Using Docker**

```bash
docker-compose up sqlserver redis -d

# Wait 30 seconds for server to start
# Connect to: Server=localhost,1433; User=sa; Password=MyP@ssw0rd123!
```

### Step 1.4: Setup Configuration Files

```bash
# Copy environment template
cp .env.example .env

# Edit .env with your settings (using Notepad++ or VS Code)
# Change sensitive values like:
# - Database connection strings
# - JWT secret key
# - Stripe keys (get from Stripe dashboard)
```

### Step 1.5: Verify Setup

```bash
# Backend
cd src/MediLink.API
dotnet run
# Should start on http://localhost:5000
# Visit http://localhost:5000/swagger for API docs

# Frontend (in another terminal)
cd frontend
npm run dev
# Should start on http://localhost:5173
```

**Estimated Time:** 2-3 hours total

---

## PHASE 2: BACKEND CORE (Weeks 1-2)

### Step 2.1: Domain Layer - Create Entities

**File:** `src/MediLink.Domain/Entities/User.cs`

```csharp
namespace MediLink.Domain.Entities;

/// <summary>
/// Base User class (polymorphic - inherited by Patient, Doctor, etc.)
/// </summary>
public abstract class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Patient entity
/// </summary>
public class Patient : User
{
    public string Phone { get; set; } = null!;
    public string? CIN { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }

    // Navigation Properties
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = [];
}

/// <summary>
/// Doctor entity (Médecin Généraliste)
/// </summary>
public class Doctor : User
{
    public string Specialization { get; set; } = null!;
    public string Cabinet { get; set; } = null!;
    public int ExperienceYears { get; set; }
    public int MaxPatientsPerDay { get; set; } = 8;

    // Navigation Properties
    public ICollection<TimeSlot> TimeSlots { get; set; } = [];
    public ICollection<BlockedDay> BlockedDays { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}

/// <summary>
/// Specialist entity (Médecin Spécialiste)
/// </summary>
public class Specialist : User
{
    public string Specialization { get; set; } = null!;
    public Money ConsultationPrice { get; set; } = null!;

    // Navigation Properties
    public ICollection<MedicalDocument> MedicalDocuments { get; set; } = [];
}

/// <summary>
/// Appointment entity
/// </summary>
public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid? MedicalDocumentId { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public Money Amount { get; set; } = null!;

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
    public MedicalDocument? MedicalDocument { get; set; }
}

/// <summary>
/// TimeSlot entity (Créneau)
/// </summary>
public class TimeSlot : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public TimeSlotStatus Status { get; set; } = TimeSlotStatus.Available;
    public Money Price { get; set; } = null!;
    public Guid? AppointmentId { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; } = null!;
    public Appointment? Appointment { get; set; }
}

/// <summary>
/// MedicalDocument entity
/// </summary>
public class MedicalDocument : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? SpecialistId { get; set; }

    public string Diagnosis { get; set; } = null!;
    public string Prescription { get; set; } = null!;
    public string? Notes { get; set; }
    public Money Total { get; set; } = null!;
    public string? PdfUrl { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Specialist? Specialist { get; set; }
    public ICollection<MedicalAct> MedicalActs { get; set; } = [];
}

/// <summary>
/// Payment entity
/// </summary>
public class Payment : BaseEntity
{
    public Guid? AppointmentId { get; set; }
    public Guid? MedicalDocumentId { get; set; }

    public Money Amount { get; set; } = null!;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? StripeTransactionId { get; set; }

    // Navigation Properties
    public Appointment? Appointment { get; set; }
    public MedicalDocument? MedicalDocument { get; set; }
}

/// <summary>
/// BlockedDay entity (Jours bloqués par médecin)
/// </summary>
public class BlockedDay : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; } = null!;
}
```

**Create Additional Entity Files:**
- `MedicalAct.cs`
- `Nurse.cs`
- ValueObjects: `Money.cs`, `Address.cs`

**Estimated Time:** 4-5 hours

### Step 2.2: Infrastructure Layer - Setup DbContext

**File:** `src/MediLink.Infrastructure/Data/AppDbContext.cs`

```csharp
namespace MediLink.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    // DbSets
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialist> Specialists { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<MedicalDocument> MedicalDocuments { get; set; }
    public DbSet<MedicalAct> MedicalActs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BlockedDay> BlockedDays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // TPH (Table-Per-Hierarchy) for User polymorphism
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Specialist>("Specialist")
            .HasValue<Nurse>("Nurse");

        // Configure Value Objects
        modelBuilder.Entity<TimeSlot>()
            .OwnsOne(ts => ts.Price);

        modelBuilder.Entity<MedicalDocument>()
            .OwnsOne(md => md.Total);

        // Indexes for performance
        modelBuilder.Entity<TimeSlot>()
            .HasIndex(ts => new { ts.DoctorId, ts.Date, ts.Status });

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.PatientId, a.Status });

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.StripeTransactionId).IsUnique();
    }
}
```

### Step 2.3: Create EF Core Migrations

```bash
cd src/MediLink.Infrastructure

# Add initial migration
dotnet ef migrations add InitialCreate --context AppDbContext

# Apply migration to database
dotnet ef database update --context AppDbContext

cd ../..
```

### Step 2.4: Create Base Repository Pattern

**File:** `src/MediLink.Infrastructure/Repositories/BaseRepository.cs`

```csharp
namespace MediLink.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Data;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await SaveChangesAsync();
    }

    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
```

**Create Specific Repositories:**
- `PatientRepository.cs`
- `DoctorRepository.cs`
- `AppointmentRepository.cs`
- `TimeSlotRepository.cs`
- `PaymentRepository.cs`

**Estimated Time:** 6-7 hours

### Step 2.5: Implement Services (Application Layer)

**File:** `src/MediLink.Application/Services/PatientService.cs`

```csharp
namespace MediLink.Application.Services;

using AutoMapper;
using FluentValidation;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;
using MediLink.Domain.Entities;
using MediLink.Infrastructure.Repositories;

public class PatientService : IPatientService
{
    private readonly IBaseRepository<Patient> _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePatientDto> _createValidator;

    public PatientService(
        IBaseRepository<Patient> repository,
        IMapper mapper,
        IValidator<CreatePatientDto> createValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<PatientDto?> GetPatientAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id);
        return patient is null ? null : _mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
    {
        var patients = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }

    public async Task<PatientDto> CreatePatientAsync(CreatePatientDto dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var patient = _mapper.Map<Patient>(dto);
        await _repository.AddAsync(patient);

        return _mapper.Map<PatientDto>(patient);
    }

    public async Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _repository.GetByIdAsync(id) 
            ?? throw new KeyNotFoundException($"Patient {id} not found");

        _mapper.Map(dto, patient);
        await _repository.UpdateAsync(patient);

        return _mapper.Map<PatientDto>(patient);
    }

    public async Task DeletePatientAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Patient {id} not found");

        await _repository.DeleteAsync(patient);
    }

    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var allPatients = await _repository.GetAllAsync();
        var filtered = allPatients
            .Where(p => p.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.LastName.Contains(query, StringComparison.OrdinalIgnoreCase)
                || (p.CIN?.Contains(query) ?? false)
                || p.Phone.Contains(query))
            .ToList();

        return _mapper.Map<IEnumerable<PatientDto>>(filtered);
    }
}
```

**Create Services for:**
- `DoctorService.cs`
- `AppointmentService.cs`
- `TimeSlotService.cs`
- `MedicalDocumentService.cs`
- `PaymentService.cs`

**Estimated Time:** 8-10 hours

### Step 2.6: Create API Controllers

**File:** `src/MediLink.API/Controllers/PatientsController.cs`

```csharp
namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediLink.Application.DTOs;
using MediLink.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var patient = await _service.GetPatientAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var patients = await _service.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto dto)
    {
        var patient = await _service.CreatePatientAsync(dto);
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, UpdatePatientDto dto)
    {
        try
        {
            var patient = await _service.UpdatePatientAsync(id, dto);
            return Ok(patient);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        try
        {
            await _service.DeletePatientAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string query)
    {
        var patients = await _service.SearchPatientsAsync(query);
        return Ok(patients);
    }
}
```

**Create Controllers for:**
- `DoctorsController.cs`
- `AppointmentsController.cs`
- `TimeSlotsController.cs`
- `MedicalDocumentsController.cs`
- `PaymentsController.cs`

**Estimated Time:** 5-6 hours

**Phase 2 Total: 1.5-2 weeks**

---

## PHASE 3: BACKEND AUTHENTICATION & ADVANCED FEATURES (Week 2-3)

### Step 3.1: Implement JWT Authentication

**File:** `src/MediLink.API/Extensions/AuthExtensions.cs`

```csharp
namespace MediLink.API.Extensions;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] 
            ?? throw new InvalidOperationException("JWT Key not configured"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        });

        return services;
    }
}
```

### Step 3.2: Create Authentication Service

**File:** `src/MediLink.Application/Services/AuthService.cs`

```csharp
namespace MediLink.Application.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using MediLink.Application.DTOs;
using MediLink.Infrastructure.Repositories;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IBaseRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = (await _userRepository.GetAllAsync())
            .FirstOrDefault(u => u.Email == request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60") * 60,
            User = new UserDto { Id = user.Id, Email = user.Email, Role = user.Role.ToString() }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = (await _userRepository.GetAllAsync())
            .FirstOrDefault(u => u.Email == request.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("User already exists");

        var patient = new Patient
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Role = UserRole.Patient,
            IsActive = true
        };

        await _userRepository.AddAsync(patient);

        var token = GenerateJwtToken(patient);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60") * 60,
            User = new UserDto { Id = patient.Id, Email = patient.Email, Role = patient.Role.ToString() }
        };
    }

    public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Implement refresh token logic
        throw new NotImplementedException();
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
```

### Step 3.3: Create Authentication Controller

**File:** `src/MediLink.API/Controllers/AuthController.cs`

```csharp
namespace MediLink.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediLink.Application.DTOs;
using MediLink.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid email or password");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(response);
    }
}
```

### Step 3.4: Implement Appointment Booking Logic

**File:** `src/MediLink.Application/Services/AppointmentService.cs`

```csharp
public class AppointmentService : IAppointmentService
{
    private readonly IBaseRepository<Appointment> _appointmentRepo;
    private readonly IBaseRepository<TimeSlot> _timeSlotRepo;
    private readonly IBaseRepository<Payment> _paymentRepo;
    private readonly IMapper _mapper;

    public async Task<AppointmentDto> BookAppointmentAsync(CreateAppointmentDto dto)
    {
        // Get time slot
        var timeSlot = await _timeSlotRepo.GetByIdAsync(dto.TimeSlotId)
            ?? throw new KeyNotFoundException("Time slot not found");

        // Check availability
        if (timeSlot.Status != TimeSlotStatus.Available)
            throw new InvalidOperationException("Time slot is not available");

        // Create appointment
        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = timeSlot.DoctorId,
            TimeSlotId = timeSlot.Id,
            Amount = timeSlot.Price,
            Status = AppointmentStatus.Scheduled
        };

        await _appointmentRepo.AddAsync(appointment);

        // Update time slot
        timeSlot.Status = TimeSlotStatus.Reserved;
        timeSlot.AppointmentId = appointment.Id;
        await _timeSlotRepo.UpdateAsync(timeSlot);

        // Create payment (advance)
        var payment = new Payment
        {
            AppointmentId = appointment.Id,
            Amount = new Money(50m, "DH"),
            Status = PaymentStatus.Pending
        };
        await _paymentRepo.AddAsync(payment);

        return _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Appointment not found");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed appointment");

        appointment.Status = AppointmentStatus.Cancelled;
        await _appointmentRepo.UpdateAsync(appointment);

        // Free up time slot
        var timeSlot = await _timeSlotRepo.GetByIdAsync(appointment.TimeSlotId);
        if (timeSlot is not null)
        {
            timeSlot.Status = TimeSlotStatus.Available;
            timeSlot.AppointmentId = null;
            await _timeSlotRepo.UpdateAsync(timeSlot);
        }
    }
}
```

### Step 3.5: Implement TimeSlot Management

**File:** `src/MediLink.Application/Services/TimeSlotService.cs`

```csharp
public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepo;
    private readonly IMapper _mapper;

    /// <summary>
    /// Generate time slots for a doctor for a given date range.
    /// Créneaux of 30 minutes from 09:00 to 17:30
    /// </summary>
    public async Task GenerateTimeSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        var timeSlots = new List<TimeSlot>();
        var currentDate = startDate.Date;
        var price = new Money(150m, "DH"); // Consultation price

        while (currentDate <= endDate.Date)
        {
            // Skip weekends
            if (currentDate.DayOfWeek != DayOfWeek.Saturday && 
                currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                // Create slots from 09:00 to 17:30
                var currentTime = new TimeSpan(9, 0, 0);
                var endTime = new TimeSpan(17, 30, 0);

                while (currentTime < endTime)
                {
                    var slot = new TimeSlot
                    {
                        DoctorId = doctorId,
                        Date = currentDate,
                        StartTime = currentTime,
                        EndTime = currentTime.Add(TimeSpan.FromMinutes(30)),
                        Status = TimeSlotStatus.Available,
                        Price = price
                    };

                    timeSlots.Add(slot);
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        await _timeSlotRepo.AddRangeAsync(timeSlots);
    }

    /// <summary>
    /// Block an entire day for a doctor
    /// </summary>
    public async Task BlockDayAsync(Guid doctorId, DateTime date, string? reason = null)
    {
        var slotsToBlock = await _timeSlotRepo
            .GetSlotsByDoctorAndDateAsync(doctorId, date.Date, date.Date.AddDays(1));

        foreach (var slot in slotsToBlock)
        {
            slot.Status = TimeSlotStatus.Blocked;
        }

        await _timeSlotRepo.UpdateRangeAsync(slotsToBlock);
    }

    /// <summary>
    /// Get available slots for doctor on a date
    /// </summary>
    public async Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
    {
        var slots = await _timeSlotRepo
            .GetAvailableSlotsByDoctorAsync(doctorId, date.Date);

        return _mapper.Map<IEnumerable<TimeSlotDto>>(slots);
    }
}
```

**Phase 3 Total: 1-1.5 weeks**

---

## PHASE 4: BACKEND STRIPE & REAL-TIME (Week 4)

### Step 4.1: Integrate Stripe Payments

**File:** `src/MediLink.Infrastructure/ExternalServices/StripeService.cs`

```csharp
namespace MediLink.Infrastructure.ExternalServices;

using Stripe;
using Stripe.Checkout;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(Guid appointmentId, string successUrl, string cancelUrl);
    Task ProcessWebhookAsync(string json, string signature);
}

public class StripeService : IStripeService
{
    private readonly IConfiguration _configuration;
    private readonly IPaymentRepository _paymentRepo;

    public StripeService(IConfiguration configuration, IPaymentRepository paymentRepo)
    {
        _configuration = configuration;
        _paymentRepo = paymentRepo;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid appointmentId, 
        string successUrl, 
        string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 5000, // 50 DH in cents
                        Currency = "mad",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "MediLink Appointment Advance Payment",
                            Description = "50 DH advance payment"
                        }
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "appointmentId", appointmentId.ToString() }
            }
        };

        var session = await new SessionService().CreateAsync(options);
        return session.Url;
    }

    public async Task ProcessWebhookAsync(string json, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json, 
                signature, 
                _configuration["Stripe:WebhookSecret"]);

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Session;
                    await HandleCheckoutSessionCompleted(session!);
                    break;

                case Events.ChargeRefunded:
                    var charge = stripeEvent.Data.Object as Charge;
                    await HandleChargeRefunded(charge!);
                    break;
            }
        }
        catch (StripeException ex)
        {
            throw new InvalidOperationException($"Stripe webhook error: {ex.Message}");
        }
    }

    private async Task HandleCheckoutSessionCompleted(Session session)
    {
        var appointmentIdMeta = session.Metadata.FirstOrDefault(m => m.Key == "appointmentId").Value;
        
        if (Guid.TryParse(appointmentIdMeta, out var appointmentId))
        {
            // Update payment status
            var payment = (await _paymentRepo.GetAllAsync())
                .FirstOrDefault(p => p.AppointmentId == appointmentId);

            if (payment is not null)
            {
                payment.Status = PaymentStatus.Completed;
                payment.StripeTransactionId = session.PaymentIntentId;
                await _paymentRepo.UpdateAsync(payment);
            }
        }
    }

    private async Task HandleChargeRefunded(Charge charge)
    {
        var payment = (await _paymentRepo.GetAllAsync())
            .FirstOrDefault(p => p.StripeTransactionId == charge.Id);

        if (payment is not null)
        {
            payment.Status = PaymentStatus.Refunded;
            await _paymentRepo.UpdateAsync(payment);
        }
    }
}
```

### Step 4.2: Create Stripe Payment Controller

**File:** `src/MediLink.API/Controllers/PaymentsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly IPaymentService _paymentService;

    [HttpPost("create-checkout-session")]
    public async Task<ActionResult> CreateCheckoutSession(Guid appointmentId)
    {
        var successUrl = $"{Request.Scheme}://{Request.Host}/payment-success";
        var cancelUrl = $"{Request.Scheme}://{Request.Host}/payment-cancel";

        var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(
            appointmentId, 
            successUrl, 
            cancelUrl);

        return Ok(new { url = checkoutUrl });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        try
        {
            await _stripeService.ProcessWebhookAsync(json, signature);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }
}
```

### Step 4.3: Implement SignalR Hubs (Real-time)

**File:** `src/MediLink.API/Hubs/AppointmentHub.cs`

```csharp
namespace MediLink.API.Hubs;

using Microsoft.AspNetCore.SignalR;

public class AppointmentHub : Hub
{
    public async Task JoinAppointmentGroup(Guid appointmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"appointment-{appointmentId}");
    }

    public async Task LeaveAppointmentGroup(Guid appointmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"appointment-{appointmentId}");
    }

    public async Task BroadcastAppointmentUpdate(Guid appointmentId, string status)
    {
        await Clients.Group($"appointment-{appointmentId}")
            .SendAsync("ReceiveAppointmentUpdate", appointmentId, status);
    }

    public async Task NotifyQueueUpdate(List<Guid> patientIds)
    {
        await Clients.Group("queue")
            .SendAsync("ReceiveQueueUpdate", patientIds);
    }
}
```

### Step 4.4: Configure SignalR in Program.cs

```csharp
// In Program.cs
builder.Services.AddSignalR();

app.MapHub<AppointmentHub>("/hubs/appointments");
app.MapHub<NotificationHub>("/hubs/notifications");
```

**Phase 4 Total: 3-4 days**

---

## PHASE 5: FRONTEND SETUP (Week 5)

### Step 5.1: Setup React Project Structure

```bash
cd frontend

# Install dependencies
npm install

# Verify setup
npm run dev
# Visit http://localhost:5173
```

### Step 5.2: Create Base Layout & Components

**File:** `frontend/src/components/Layout/Header.tsx`

```typescript
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';

export const Header: React.FC = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="bg-blue-600 text-white p-4">
      <div className="max-w-7xl mx-auto flex justify-between items-center">
        <h1 className="text-2xl font-bold">🏥 MediLink</h1>
        <div className="flex items-center gap-4">
          {user && (
            <>
              <span>{user.email}</span>
              <button
                onClick={handleLogout}
                className="bg-red-500 px-4 py-2 rounded"
              >
                Logout
              </button>
            </>
          )}
        </div>
      </div>
    </header>
  );
};
```

### Step 5.3: Create Authentication Pages

**File:** `frontend/src/pages/auth/LoginPage.tsx`

```typescript
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import toast from 'react-hot-toast';

export const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      await login(formData.email, formData.password);
      toast.success('Login successful!');
      navigate('/dashboard');
    } catch (error) {
      toast.error('Login failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-md w-96">
        <h2 className="text-2xl font-bold mb-6 text-center">MediLink Login</h2>
        
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium mb-2">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              required
            />
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium mb-2">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>

        <p className="mt-4 text-center text-sm">
          Don't have an account?{' '}
          <a href="/register" className="text-blue-600 hover:underline">
            Register here
          </a>
        </p>
      </div>
    </div>
  );
};
```

### Step 5.4: Create Hooks for Data Fetching

**File:** `frontend/src/hooks/useAuth.ts`

```typescript
import { useContext } from 'react';
import { AuthContext } from '@/context/AuthContext';

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
```

### Step 5.5: Setup Context for State Management

**File:** `frontend/src/context/AuthContext.tsx`

```typescript
import React, { createContext, useState, useEffect } from 'react';
import { authService } from '@/services/authService';

export interface User {
  id: string;
  email: string;
  role: string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, firstName: string, lastName: string) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in on mount
    const token = localStorage.getItem('token');
    if (token) {
      // Validate token and fetch user
      try {
        const userData = JSON.parse(localStorage.getItem('user') || '{}');
        setUser(userData);
      } catch (error) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
    setLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    const response = await authService.login(email, password);
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
    setUser(response.user);
  };

  const register = async (email: string, password: string, firstName: string, lastName: string) => {
    const response = await authService.register(email, password, firstName, lastName);
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
    setUser(response.user);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
```

**Phase 5 Total: 3-4 days**

---

## PHASE 6: FRONTEND FEATURES (Weeks 5-6)

### Step 6.1: Appointment Booking Page

**File:** `frontend/src/pages/Appointments/BookingPage.tsx`

```typescript
import React, { useState } from 'react';
import { useQuery, useMutation } from '@tanstack/react-query';
import { appointmentService } from '@/services/appointmentService';
import { doctorService } from '@/services/doctorService';
import Calendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';

export const BookingPage: React.FC = () => {
  const [selectedDoctorId, setSelectedDoctorId] = useState<string | null>(null);
  const [selectedSlotId, setSelectedSlotId] = useState<string | null>(null);

  // Fetch doctors
  const { data: doctors, isLoading: doctorsLoading } = useQuery({
    queryKey: ['doctors'],
    queryFn: () => doctorService.getDoctors(),
  });

  // Fetch available slots
  const { data: slots, isLoading: slotsLoading } = useQuery({
    queryKey: ['slots', selectedDoctorId],
    queryFn: () => doctorService.getAvailableSlots(selectedDoctorId!, new Date()),
    enabled: !!selectedDoctorId,
  });

  // Book appointment mutation
  const bookMutation = useMutation({
    mutationFn: () => appointmentService.bookAppointment({
      doctorId: selectedDoctorId!,
      timeSlotId: selectedSlotId!,
    }),
    onSuccess: () => {
      toast.success('Appointment booked successfully!');
      // Redirect to payment
    },
  });

  return (
    <div className="max-w-6xl mx-auto p-4">
      <h1 className="text-3xl font-bold mb-6">Book an Appointment</h1>

      {/* Doctor Selection */}
      <div className="mb-6">
        <h2 className="text-xl font-bold mb-4">Select a Doctor</h2>
        <div className="grid grid-cols-3 gap-4">
          {doctors?.map(doctor => (
            <div
              key={doctor.id}
              onClick={() => setSelectedDoctorId(doctor.id)}
              className={`p-4 border rounded cursor-pointer ${
                selectedDoctorId === doctor.id ? 'border-blue-600 bg-blue-50' : ''
              }`}
            >
              <h3 className="font-bold">{doctor.firstName} {doctor.lastName}</h3>
              <p className="text-sm text-gray-600">{doctor.specialization}</p>
              <p className="text-sm text-gray-600">{doctor.cabinet}</p>
            </div>
          ))}
        </div>
      </div>

      {/* Time Slots Selection */}
      {selectedDoctorId && (
        <div className="mb-6">
          <h2 className="text-xl font-bold mb-4">Select a Time Slot</h2>
          <div className="grid grid-cols-4 gap-2">
            {slots?.map(slot => (
              <button
                key={slot.id}
                onClick={() => setSelectedSlotId(slot.id)}
                className={`p-3 border rounded ${
                  selectedSlotId === slot.id ? 'bg-blue-600 text-white' : ''
                }`}
              >
                {slot.startTime} - {slot.endTime}
              </button>
            ))}
          </div>
        </div>
      )}

      {/* Book Button */}
      {selectedSlotId && (
        <button
          onClick={() => bookMutation.mutate()}
          disabled={bookMutation.isPending}
          className="bg-blue-600 text-white px-6 py-3 rounded"
        >
          {bookMutation.isPending ? 'Booking...' : 'Book Appointment'}
        </button>
      )}
    </div>
  );
};
```

### Step 6.2: Medical Documents Page

**File:** `frontend/src/pages/Medical/MedicalDocumentsPage.tsx`

```typescript
import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { medicalService } from '@/services/medicalService';
import { useAuth } from '@/hooks/useAuth';

export const MedicalDocumentsPage: React.FC = () => {
  const { user } = useAuth();
  
  const { data: documents, isLoading } = useQuery({
    queryKey: ['medical-documents', user?.id],
    queryFn: () => medicalService.getPatientDocuments(user!.id),
  });

  const handleDownloadPDF = async (documentId: string) => {
    const pdfData = await medicalService.downloadPDF(documentId);
    // Trigger download
  };

  return (
    <div className="max-w-6xl mx-auto p-4">
      <h1 className="text-3xl font-bold mb-6">Medical Documents</h1>

      {isLoading ? (
        <p>Loading...</p>
      ) : (
        <div className="space-y-4">
          {documents?.map(doc => (
            <div key={doc.id} className="border p-4 rounded-lg">
              <div className="flex justify-between items-start">
                <div>
                  <h3 className="font-bold">Diagnosis: {doc.diagnosis}</h3>
                  <p className="text-sm text-gray-600">Doctor: {doc.doctorName}</p>
                  <p className="text-sm text-gray-600">Date: {new Date(doc.createdAt).toLocaleDateString()}</p>
                </div>
                <button
                  onClick={() => handleDownloadPDF(doc.id)}
                  className="bg-blue-600 text-white px-4 py-2 rounded"
                >
                  Download PDF
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
```

### Step 6.3: Create API Service Layer

**File:** `frontend/src/services/api.ts`

```typescript
import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

export const api: AxiosInstance = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

**Phase 6 Total: 5-6 days**

---

## PHASE 7: INTEGRATION & REAL-TIME (Week 7)

### Step 7.1: Setup SignalR Client

**File:** `frontend/src/services/signalrService.ts`

```typescript
import * as signalR from '@microsoft/signalr';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${API_BASE_URL}/hubs/appointments`)
  .withAutomaticReconnect()
  .build();

export const signalRService = {
  start: async () => {
    try {
      await connection.start();
      console.log('SignalR connected');
    } catch (err) {
      console.error('SignalR connection failed:', err);
    }
  },

  joinAppointmentGroup: (appointmentId: string) => {
    connection.invoke('JoinAppointmentGroup', appointmentId);
  },

  onAppointmentUpdate: (callback: (id: string, status: string) => void) => {
    connection.on('ReceiveAppointmentUpdate', callback);
  },

  stop: async () => {
    await connection.stop();
  },
};
```

### Step 7.2: Stripe Payment Integration

**File:** `frontend/src/pages/Payment/PaymentPage.tsx`

```typescript
import React from 'react';
import { loadStripe } from '@stripe/stripe-js';
import { EmbeddedCheckoutProvider, EmbeddedCheckout } from '@stripe/react-stripe-js';
import { paymentService } from '@/services/paymentService';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY);

export const PaymentPage: React.FC = () => {
  const appointmentId = new URLSearchParams(window.location.search).get('appointmentId');

  const fetchClientSecret = async () => {
    const response = await paymentService.createCheckoutSession(appointmentId!);
    return response.clientSecret;
  };

  return (
    <EmbeddedCheckoutProvider stripe={stripePromise} options={{ fetchClientSecret }}>
      <EmbeddedCheckout />
    </EmbeddedCheckoutProvider>
  );
};
```

**Phase 7 Total: 3-4 days**

---

## PHASE 8: TESTING & DEPLOYMENT (Week 8)

### Step 8.1: Write Unit Tests (Backend)

```bash
cd tests/MediLink.UnitTests

# Create test files
# Tests for Services
# Tests for Repositories
# Tests for Validators

dotnet test
```

### Step 8.2: Write Integration Tests

```bash
cd tests/MediLink.IntegrationTests

# Create API endpoint tests
# Database integration tests
# Stripe integration tests

dotnet test
```

### Step 8.3: Frontend Testing

```bash
cd frontend

# Unit tests with Vitest
npm run test

# E2E tests with Playwright (optional)
npx playwright test
```

### Step 8.4: Docker Build & Run

```bash
# Build and run with Docker Compose
docker-compose up --build

# Check services
docker ps

# View logs
docker-compose logs -f
```

**Phase 8 Total: 3-4 days**

---

## 📋 COMPLETE TIMELINE SUMMARY

| Phase | Duration | Start | End |
|-------|----------|-------|-----|
| 1. Setup & Prerequisites | 1-2 days | Day 1 | Day 2 |
| 2. Backend Core | 1.5-2 weeks | Day 3 | Day 16 |
| 3. Backend Auth & Features | 1-1.5 weeks | Day 17 | Day 27 |
| 4. Stripe & Real-time | 3-4 days | Day 28 | Day 31 |
| 5. Frontend Setup | 3-4 days | Day 32 | Day 35 |
| 6. Frontend Features | 5-6 days | Day 36 | Day 41 |
| 7. Integration | 3-4 days | Day 42 | Day 45 |
| 8. Testing & Deploy | 3-4 days | Day 46 | Day 49 |
| **Total** | **~8-10 weeks** | | |

---

## 🔧 INSTALLATION CHECKLIST

### Prerequisites Installation
- [ ] Visual Studio 2022 or VS Code
- [ ] .NET 8 SDK
- [ ] SQL Server / LocalDB
- [ ] Node.js 18+
- [ ] Git
- [ ] Docker Desktop (optional)
- [ ] Postman (optional)

### Development Tools
- [ ] VS Code Extensions (C#, REST Client, Prettier)
- [ ] Stripe Account (get API keys)
- [ ] SendGrid Account (email)
- [ ] Azure Account (optional for deployment)

### Initial Setup
- [ ] Clone repository
- [ ] Run `dotnet restore`
- [ ] Setup database
- [ ] Create `.env` file
- [ ] Run frontend `npm install`

---

## 🚀 GETTING STARTED NOW

1. **Complete Phase 1 (Setup)** first - ensure all tools are installed
2. **Follow backend phases sequentially** (2 → 3 → 4)
3. **Then start frontend** (5 → 6 → 7)
4. **Finish with testing & deployment** (8)

This ensures dependencies are met and integration is smooth!

---

**Ready to start building MediLink?** 🏥✨

Let me know which phase you want to begin with!
