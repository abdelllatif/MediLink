# 🏛️ ARCHITECTURE - MediLink

## Architecture Générale

MediLink suit une **Clean Architecture** avec séparation claire des responsabilités en 4 couches:

```
┌─────────────────────────────────────────┐
│         API Layer (Controllers)         │
│         (REST Endpoints)                │
└──────────────────┬──────────────────────┘
                   │ DTOs
┌──────────────────▼──────────────────────┐
│    Application Layer (Services)         │
│    (Business Logic, Mappings)           │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│       Domain Layer (Entities)           │
│    (Business Rules, ValueObjects)       │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│    Infrastructure Layer (Data, Ext)     │
│    (DB, Repositories, External APIs)    │
└─────────────────────────────────────────┘
```

---

## 1. API Layer (Présentation)

### Responsabilités
- Exposer endpoints REST
- Valider les requêtes HTTP
- Transformer DTO en réponses JSON
- Gérer authentification JWT

### Structure

```
MediLink.API/
├── Controllers/
│   ├── AuthController.cs          # Login, Register, Refresh Token
│   ├── PatientsController.cs       # CRUD Patients
│   ├── DoctorsController.cs        # CRUD Doctors
│   ├── AppointmentsController.cs   # Rendez-vous
│   ├── TimeSlotsController.cs      # Gestion créneaux
│   ├── MedicalDocumentsController.cs
│   ├── PaymentsController.cs       # Intégration Stripe
│   └── SpecialistsController.cs    # Spécialistes
│
├── Middlewares/
│   ├── ExceptionMiddleware.cs      # Gestion erreurs globale
│   └── JwtMiddleware.cs            # Validation JWT tokens
│
├── Extensions/
│   ├── ServiceExtensions.cs        # DI Configuration
│   ├── SwaggerExtensions.cs        # Swagger/OpenAPI setup
│   └── AuthExtensions.cs           # Auth Configuration
│
├── Hubs/
│   ├── AppointmentHub.cs           # SignalR real-time
│   └── NotificationHub.cs
│
├── Program.cs                      # Startup Configuration
└── appsettings.json
```

### Endpoints Exemple

```csharp
// Authentication
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh-token

// Patients
GET    /api/patients
GET    /api/patients/{id}
POST   /api/patients
PUT    /api/patients/{id}
DELETE /api/patients/{id}

// Time Slots
GET    /api/timeslots?doctorId={id}&date={date}
GET    /api/timeslots/{id}
POST   /api/timeslots

// Appointments (Reservations)
GET    /api/appointments
POST   /api/appointments
PUT    /api/appointments/{id}/cancel
GET    /api/appointments/{id}/medical-document

// Payments
POST   /api/payments/create-checkout-session
GET    /api/payments/{id}
POST   /api/webhooks/stripe  # Webhook Stripe

// Medical Documents
GET    /api/medical-documents/{id}
POST   /api/medical-documents/{id}/download
```

---

## 2. Application Layer (Services)

### Responsabilités
- Implémentation de la logique métier
- Orchestration entre services
- Mapping DTO ↔ Entity (AutoMapper)
- Validation avec FluentValidation

### Services Principaux

```csharp
// Services
IPatientService
  ├── GetPatientAsync(patientId)
  ├── CreatePatientAsync(dto)
  ├── UpdatePatientAsync(id, dto)
  ├── SearchPatientsAsync(query)
  └── GetPatientHistoryAsync(patientId)

IAppointmentService
  ├── BookAppointmentAsync(dto)
  ├── CancelAppointmentAsync(id)
  ├── GetAppointmentsAsync(filter)
  ├── CompleteAppointmentAsync(id)
  └── GetAppointmentDocumentAsync(id)

IDoctorService
  ├── GetDoctorAsync(doctorId)
  ├── GetAvailableTimeSlotsAsync(doctorId, date)
  ├── BlockDayAsync(doctorId, date, reason)
  ├── UnblockDayAsync(doctorId, date)
  └── GetDoctorScheduleAsync(doctorId)

IPaymentService
  ├── CreateCheckoutSessionAsync(appointmentId)
  ├── ProcessWebhookAsync(stripeEvent)
  ├── GetPaymentAsync(paymentId)
  └── GenerateInvoiceAsync(paymentId)

IMedicalDocumentService
  ├── CreateDocumentAsync(consultationData)
  ├── GetDocumentAsync(documentId)
  ├── GeneratePdfAsync(documentId)
  └── ArchiveDocumentAsync(documentId)
```

### Mapping (AutoMapper)

```csharp
// MappingProfile.cs
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient
        CreateMap<Patient, PatientDto>().ReverseMap();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) 
                => srcMember != null));

        // Appointment
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<CreateAppointmentDto, Appointment>();
        
        // TimeSlot
        CreateMap<TimeSlot, TimeSlotDto>();
        
        // MedicalDocument
        CreateMap<MedicalDocument, MedicalDocumentDto>();
    }
}
```

---

## 3. Domain Layer (Entités)

### Responsabilités
- Définir les entités métier
- Implémenter la logique métier interne
- ValueObjects pour concepts clés
- Énumérations pour statuts

### Entités Principales

```csharp
namespace MediLink.Domain.Entities;

// User (Classe de base polymorphe)
public abstract class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Patient
public class Patient : User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string? CIN { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
    
    public ICollection<Appointment> Appointments { get; set; }
    public ICollection<MedicalDocument> MedicalDocuments { get; set; }
}

// Doctor (Généraliste)
public class Doctor : User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public string Cabinet { get; set; }
    public int ExperienceYears { get; set; }
    public int MaxPatientsPerDay { get; set; }
    
    public ICollection<TimeSlot> TimeSlots { get; set; }
    public ICollection<BlockedDay> BlockedDays { get; set; }
    public ICollection<Appointment> Appointments { get; set; }
}

// Specialist
public class Specialist : User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public Money ConsultationPrice { get; set; }
    
    public ICollection<MedicalDocument> MedicalDocuments { get; set; }
}

// Appointment (Rendez-vous)
public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid? MedicalDocumentId { get; set; }
    
    public AppointmentStatus Status { get; set; }
    public Money Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public MedicalDocument? MedicalDocument { get; set; }
}

// TimeSlot (Créneau)
public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public TimeSlotStatus Status { get; set; }
    public Money Price { get; set; }
    public Guid? AppointmentId { get; set; }
    
    // Navigation
    public Doctor Doctor { get; set; }
    public Appointment? Appointment { get; set; }
}

// MedicalDocument
public class MedicalDocument
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? SpecialistId { get; set; }
    
    public string Diagnosis { get; set; }
    public string Prescription { get; set; }
    public string? Notes { get; set; }
    public Money Total { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public Specialist? Specialist { get; set; }
    public ICollection<MedicalAct> MedicalActs { get; set; }
}

// MedicalAct
public class MedicalAct
{
    public Guid Id { get; set; }
    public Guid MedicalDocumentId { get; set; }
    public MedicalActType Type { get; set; }
    public Money Price { get; set; }
    public string? Notes { get; set; }
    
    public MedicalDocument MedicalDocument { get; set; }
}

// Payment
public class Payment
{
    public Guid Id { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? MedicalDocumentId { get; set; }
    
    public Money Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? StripeTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Appointment? Appointment { get; set; }
    public MedicalDocument? MedicalDocument { get; set; }
}

// BlockedDay
public class BlockedDay
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    
    public Doctor Doctor { get; set; }
}
```

### ValueObjects

```csharp
// Money.cs
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "DH";
    
    public Money(decimal amount, string currency = "DH")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        Amount = amount;
        Currency = currency;
    }
    
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new ArgumentException("Cannot add different currencies");
        
        return new Money(a.Amount + b.Amount, a.Currency);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

// Address.cs
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string? Country { get; private set; } = "Morocco";
    
    public Address(string street, string city, string postalCode)
    {
        Street = street ?? throw new ArgumentException(nameof(street));
        City = city ?? throw new ArgumentException(nameof(city));
        PostalCode = postalCode ?? throw new ArgumentException(nameof(postalCode));
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}
```

### Énumérations

```csharp
namespace MediLink.Domain.Enums;

public enum UserRole
{
    Admin = 0,
    Generalist = 1,      // Médecin généraliste
    Specialist = 2,      // Médecin spécialiste
    Nurse = 3,           // Infirmier
    Patient = 4
}

public enum AppointmentStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    PendingPayment = 4,
    NoShow = 5
}

public enum TimeSlotStatus
{
    Available = 0,       // LIBRE
    Reserved = 1,        // RÉSERVÉ
    Blocked = 2          // BLOQUÉ
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum MedicalActType
{
    IRM = 0,
    Radiography = 1,
    ECG = 2,
    BloodTest = 3,
    UrineTest = 4,
    Ultrasound = 5,
    CTScan = 6
}

public enum PriorityLevel
{
    Urgent = 0,
    Normal = 1,
    NonUrgent = 2
}
```

---

## 4. Infrastructure Layer (Données & Externe)

### Responsabilités
- Accès à la base de données (EF Core)
- Implémentation des repositories
- Intégrations externes (Stripe, Email, PDF)

### Structure

```
MediLink.Infrastructure/
├── Data/
│   ├── AppDbContext.cs             # DbContext EF Core
│   ├── Configurations/
│   │   ├── PatientConfig.cs
│   │   ├── DoctorConfig.cs
│   │   ├── TimeSlotConfig.cs
│   │   ├── AppointmentConfig.cs
│   │   └── MedicalDocumentConfig.cs
│   └── Migrations/
│       └── (EF Core migrations)
│
├── Repositories/
│   ├── IBaseRepository.cs
│   ├── BaseRepository.cs
│   ├── PatientRepository.cs
│   ├── AppointmentRepository.cs
│   ├── TimeSlotRepository.cs
│   ├── DoctorRepository.cs
│   └── MedicalDocumentRepository.cs
│
├── ExternalServices/
│   ├── StripeService.cs
│   ├── PdfService.cs
│   ├── EmailService.cs
│   └── SmsService.cs (future)
│
├── DependencyInjection.cs          # Extension method DI
└── MediLink.Infrastructure.csproj
```

### DbContext Configuration

```csharp
// AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialist> Specialists { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalDocument> MedicalDocuments { get; set; }
    public DbSet<MedicalAct> MedicalActs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BlockedDay> BlockedDays { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
        // ... other configurations
        
        // Index pour performance
        modelBuilder.Entity<TimeSlot>()
            .HasIndex(ts => new { ts.DoctorId, ts.Date, ts.Status });
        
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.PatientId, a.Status });
    }
}
```

### Repository Pattern

```csharp
// IBaseRepository.cs
public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}

// BaseRepository.cs
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

// TimeSlotRepository.cs
public class TimeSlotRepository : BaseRepository<TimeSlot>
{
    public TimeSlotRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAsync(
        Guid doctorId, DateTime date)
    {
        return await _dbSet
            .Where(ts => ts.DoctorId == doctorId 
                && ts.Date.Date == date.Date 
                && ts.Status == TimeSlotStatus.Available)
            .OrderBy(ts => ts.StartTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TimeSlot>> GetSlotsByDoctorAndDateRangeAsync(
        Guid doctorId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(ts => ts.DoctorId == doctorId 
                && ts.Date >= startDate 
                && ts.Date <= endDate)
            .OrderBy(ts => ts.Date)
            .ThenBy(ts => ts.StartTime)
            .ToListAsync();
    }
}
```

### External Services

```csharp
// StripeService.cs
public class StripeService : IStripeService
{
    private readonly IStripeClient _client;
    
    public StripeService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = 
            config["Stripe:SecretKey"];
        _client = new StripeClient();
    }
    
    public async Task<string> CreateCheckoutSessionAsync(
        CreatePaymentRequest request)
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
                        UnitAmount = (long)(request.Amount * 100),
                        Currency = "mad",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.Description,
                        }
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
        };
        
        var session = await new SessionService()
            .CreateAsync(options);
        
        return session.Url;
    }
}

// PdfService.cs
public class PdfService : IPdfService
{
    public async Task<byte[]> GenerateMedicalReportAsync(
        MedicalDocument document)
    {
        var document_pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(2, Unit.Centimetre);
                
                page.Header().Text("RAPPORT MÉDICAL")
                    .FontSize(20)
                    .Bold();
                
                page.Content().Text($"Patient: {document.Patient.FirstName} " +
                    $"{document.Patient.LastName}")
                    .FontSize(12);
                
                page.Content().Text($"Diagnostic: {document.Diagnosis}")
                    .FontSize(11);
                
                page.Content().Text($"Prescription: {document.Prescription}")
                    .FontSize(11);
            });
        });
        
        return await document_pdf.GeneratePdfAsync();
    }
}

// EmailService.cs
public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    
    public async Task SendConfirmationEmailAsync(Patient patient)
    {
        var from = new EmailAddress("noreply@medilink.com", "MediLink");
        var to = new EmailAddress(patient.Email, patient.FirstName);
        var subject = "Confirmation de votre compte MediLink";
        var htmlContent = $"<h1>Bienvenue {patient.FirstName}!</h1>";
        
        var msg = new SendGridMessage()
        {
            From = from,
            Subject = subject,
            HtmlContent = htmlContent,
        };
        msg.AddTo(to);
        
        await _client.SendEmailAsync(msg);
    }
}
```

---

## 5. Shared Layer (Utilitaires)

```
MediLink.Shared/
├── Constants/
│   ├── Roles.cs
│   ├── Prices.cs
│   └── AppConstants.cs
│
└── Helpers/
    ├── DateHelper.cs
    ├── CalculationHelper.cs
    └── ValidationHelper.cs
```

---

## 6. Frontend Architecture (React)

```
frontend/src/
├── pages/
│   ├── auth/
│   │   ├── LoginPage.tsx
│   │   └── RegisterPage.tsx
│   ├── dashboard/
│   │   ├── DashboardPage.tsx
│   │   └── components/
│   ├── appointments/
│   │   ├── AppointmentsPage.tsx
│   │   ├── BookingPage.tsx
│   │   └── components/
│   ├── doctors/
│   │   ├── DoctorsPage.tsx
│   │   └── DoctorDetailPage.tsx
│   ├── medical/
│   │   └── MedicalDocumentsPage.tsx
│   └── admin/
│       └── AdminPage.tsx
│
├── components/
│   ├── common/
│   │   ├── Header.tsx
│   │   ├── Sidebar.tsx
│   │   ├── Footer.tsx
│   │   └── ProtectedRoute.tsx
│   ├── calendar/
│   │   ├── Calendar.tsx
│   │   └── TimeSlotGrid.tsx
│   ├── forms/
│   │   ├── PatientForm.tsx
│   │   ├── BookingForm.tsx
│   │   └── ConsultationForm.tsx
│   └── ui/
│       ├── Button.tsx
│       ├── Modal.tsx
│       ├── Card.tsx
│       └── ...
│
├── services/
│   ├── api.ts           # Axios instance
│   ├── authService.ts
│   ├── appointmentService.ts
│   ├── doctorService.ts
│   ├── paymentService.ts
│   └── medicalService.ts
│
├── hooks/
│   ├── useAuth.ts
│   ├── useAppointments.ts
│   ├── useDoctors.ts
│   ├── usePayments.ts
│   └── useQuery.ts
│
├── context/
│   ├── AuthContext.tsx
│   └── NotificationContext.tsx
│
├── utils/
│   ├── formatters.ts    # Formatage dates, nombres
│   ├── validators.ts    # Validation formulaires
│   ├── constants.ts
│   └── helpers.ts
│
├── types/
│   ├── index.ts
│   ├── entities.ts
│   └── api.ts
│
├── App.tsx
├── main.tsx
└── styles/
    └── globals.css      # Tailwind + custom CSS
```

### State Management (React Query)

```typescript
// hooks/useAppointments.ts
export function useAppointments() {
  return useQuery({
    queryKey: ['appointments'],
    queryFn: async () => {
      const { data } = await api.get('/appointments');
      return data;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
  });
}

export function useCreateAppointment() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (appointmentData) => {
      const { data } = await api.post('/appointments', appointmentData);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
    },
  });
}
```

---

## 7. Communication API

### REST API Pattern

```
GET    /api/v1/patients
POST   /api/v1/patients
GET    /api/v1/patients/{id}
PUT    /api/v1/patients/{id}
DELETE /api/v1/patients/{id}

GET    /api/v1/appointments?patientId={id}&status={status}
POST   /api/v1/appointments
GET    /api/v1/appointments/{id}
PUT    /api/v1/appointments/{id}
DELETE /api/v1/appointments/{id}
```

### SignalR Real-time

```csharp
// Backend Hub
public class AppointmentHub : Hub
{
    public async Task JoinAppointmentGroup(Guid appointmentId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, 
            $"appointment-{appointmentId}");
    }
    
    public async Task BroadcastAppointmentUpdate(
        Guid appointmentId, 
        AppointmentStatus status)
    {
        await Clients.Group($"appointment-{appointmentId}")
            .SendAsync("ReceiveAppointmentUpdate", appointmentId, status);
    }
}

// Frontend Hook
export function useAppointmentUpdates(appointmentId: string) {
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/appointments")
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveAppointmentUpdate", (id, status) => {
      // Update UI
    });

    connection.start();
    connection.invoke("JoinAppointmentGroup", appointmentId);

    return () => connection.stop();
  }, [appointmentId]);
}
```

---

## 8. Database Relationships

```sql
-- User Hierarchy
Users (base table with discriminator)
  ├── Patients
  ├── Doctors
  ├── Specialists
  └── Nurses

-- Core Tables
Patients ←→ Appointments
  ↓
MedicalDocuments ←→ MedicalActs

Doctors ←→ TimeSlots ←→ Appointments
Doctors ←→ BlockedDays

Specialists ←→ MedicalDocuments

-- Financial
Appointments ←→ Payments
MedicalDocuments ←→ Payments
```

---

## 9. Deployment Architecture

```
┌─────────────────────────────────┐
│      Client Browser             │
│  (React App - Vite Build)       │
└────────────┬────────────────────┘
             │ HTTPS
  ┌──────────▼──────────┐
  │  CDN / Static Host  │
  │ (Azure Static Apps) │
  └─────────────────────┘

┌─────────────────────────────────┐
│     Load Balancer               │
│    (Application Gateway)        │
└────────────┬────────────────────┘
             │ HTTPS
  ┌──────────▼──────────┐
  │ API Instances       │
  │ (.NET + ASP.Core)   │
  │ (Auto-scaling)      │
  └──────────┬──────────┘
             │
  ┌──────────▼──────────┐
  │  Azure SQL Database │
  │  (SQL Server)       │
  └─────────────────────┘

┌─────────────────────────────────┐
│  External Services              │
├─────────────────────────────────┤
│ - Stripe (Payments)             │
│ - SendGrid (Email)              │
│ - Azure Storage (PDF/Files)     │
└─────────────────────────────────┘
```

---

## 10. Testing Architecture

### Unit Tests (xUnit + Moq)

```csharp
[Fact]
public async Task BookAppointment_WithValidData_ReturnsSuccess()
{
    // Arrange
    var mockRepo = new Mock<IAppointmentRepository>();
    var service = new AppointmentService(mockRepo.Object);
    
    // Act
    var result = await service.BookAppointmentAsync(/* data */);
    
    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(r => r.AddAsync(It.IsAny<Appointment>()), 
        Times.Once);
}
```

### Integration Tests

```csharp
public class AppointmentControllerTests : IAsyncLifetime
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client;
    
    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task BookAppointment_ReturnsOk()
    {
        // Arrange
        var command = new BookAppointmentRequest { /* */ };
        
        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/appointments", 
            command);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

---

## 11. Sécurité & CORS

### JWT Configuration

```csharp
// Startup
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });
```

### CORS Policy

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

---

**Document d'Architecture - MediLink v1.0**  
Date: 2024

