# 🤝 Guides de Contribution - MediLink

Merci de contribuer à MediLink! Ce document vous guidera sur comment contribuer efficacement au projet.

## Code de Conduite

Tous les contributeurs doivent adhérer à un comportement respectueux et professionnel. 

---

## Comment Contribuer

### 1. Forker et Cloner

```bash
# Fork le repo via GitHub UI

# Cloner votre fork
git clone https://github.com/YOUR_USERNAME/MediLink.git
cd MediLink

# Ajouter le repo upstream
git remote add upstream https://github.com/original-repo/MediLink.git
```

### 2. Créer une Branche Feature

```bash
# Mettre à jour depuis upstream
git fetch upstream
git checkout -b feature/JIRA-XXX-description upstream/main

# Convention de nommage:
# - feature/JIRA-XXX-description (nouvelles fonctionnalités)
# - bugfix/JIRA-XXX-description (corrections de bugs)
# - hotfix/JIRA-XXX-description (corrections urgentes)
# - docs/JIRA-XXX-description (documentation)
```

### 3. Committer vos Changements

```bash
# Commit avec messages clairs et descriptifs
git commit -m "feat(JIRA-XXX): Add patient appointment feature"
git commit -m "fix(JIRA-YYY): Resolve timezone issue in TimeSlot"

# Format: type(scope): subject
# Types: feat, fix, docs, style, refactor, test, chore
```

### 4. Pousser et Créer une PR

```bash
git push origin feature/JIRA-XXX-description
```

Ensuite, créez une Pull Request via GitHub UI.

---

## Standards de Code

### C# Backend

#### Style & Conventions

```csharp
// ✅ CORRECT
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;
    
    public PatientService(IPatientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<PatientDto?> GetPatientAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id);
        return _mapper.Map<PatientDto>(patient);
    }
}

// ❌ INCORRECT
public class PatientService
{
    private IPatientRepository repo; // Bad naming
    
    public PatientDto GetPatient(Guid id) // Missing async
    {
        var p = repo.GetById(id); // Bad variable name
        return PatientDto.FromEntity(p);
    }
}
```

#### Règles SOLID

```csharp
// ✅ Single Responsibility
public class AppointmentService : IAppointmentService
{
    // Seule responsabilité: gestion des rendez-vous
}

public class PaymentService : IPaymentService
{
    // Seule responsabilité: gestion des paiements
}

// ✅ Dependency Injection
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _repository;
    private readonly ITimeSlotService _timeSlotService;
    
    public DoctorService(
        IDoctorRepository repository,
        ITimeSlotService timeSlotService)
    {
        _repository = repository;
        _timeSlotService = timeSlotService;
    }
}
```

#### Null Handling

```csharp
// ✅ CORRECT - Nullable reference types enabled
public class Patient
{
    public string Email { get; set; } = null!; // Required
    public string? Bio { get; set; } // Optional
}

public async Task<PatientDto?> GetPatientAsync(Guid id)
{
    var patient = await _repository.GetByIdAsync(id);
    return patient is not null ? _mapper.Map<PatientDto>(patient) : null;
}
```

### TypeScript/React Frontend

#### Style & Conventions

```typescript
// ✅ CORRECT
interface IAppointment {
  id: string;
  patientId: string;
  status: AppointmentStatus;
  createdAt: Date;
}

const AppointmentCard: React.FC<{ appointment: IAppointment }> = ({ appointment }) => {
  const [loading, setLoading] = useState(false);
  
  const handleBook = async () => {
    setLoading(true);
    try {
      await appointmentService.book(appointment.id);
    } catch (error) {
      console.error('Booking failed:', error);
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <div className="appointment-card">
      <h3>{appointment.id}</h3>
      <button onClick={handleBook} disabled={loading}>
        {loading ? 'Booking...' : 'Book'}
      </button>
    </div>
  );
};
```

#### Component Organization

```typescript
// pages/appointments/BookingPage.tsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { BookingForm } from '@/components/forms/BookingForm';
import { appointmentService } from '@/services/appointmentService';

export const BookingPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  
  const handleSubmit = async (data: IBookingRequest) => {
    setLoading(true);
    try {
      const result = await appointmentService.book(data);
      navigate(`/appointments/${result.id}`);
    } finally {
      setLoading(false);
    }
  };
  
  return <BookingForm onSubmit={handleSubmit} isLoading={loading} />;
};
```

---

## Testing

### Unit Tests (Backend)

```csharp
[Fact]
public async Task BookAppointment_WithValidData_ReturnsAppointmentId()
{
    // Arrange
    var mockRepository = new Mock<IAppointmentRepository>();
    var mockMapper = new Mock<IMapper>();
    var service = new AppointmentService(mockRepository.Object, mockMapper.Object);
    
    var request = new BookAppointmentRequest
    {
        PatientId = Guid.NewGuid(),
        TimeSlotId = Guid.NewGuid(),
    };
    
    var appointment = new Appointment { Id = Guid.NewGuid() };
    mockMapper.Setup(m => m.Map<Appointment>(It.IsAny<BookAppointmentRequest>()))
        .Returns(appointment);
    
    // Act
    var result = await service.BookAppointmentAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(appointment.Id, result.Id);
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Once);
}

[Theory]
[InlineData(null)]
[InlineData("")]
public void ValidateEmail_WithInvalidEmail_ThrowsException(string email)
{
    // Act & Assert
    Assert.Throws<ValidationException>(() => 
        ValidationHelper.ValidateEmail(email));
}
```

### Component Tests (Frontend)

```typescript
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BookingForm } from './BookingForm';

describe('BookingForm', () => {
  it('should submit form with valid data', async () => {
    const user = userEvent.setup();
    const mockOnSubmit = vi.fn();
    
    render(<BookingForm onSubmit={mockOnSubmit} />);
    
    await user.type(screen.getByLabelText(/date/i), '2024-12-25');
    await user.click(screen.getByRole('button', { name: /submit/i }));
    
    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalled();
    });
  });
});
```

---

## Documentation

### Documenter le Code

```csharp
/// <summary>
/// Books an appointment for a patient with a doctor.
/// </summary>
/// <param name="request">The booking request containing patient and time slot details.</param>
/// <returns>The created appointment ID.</returns>
/// <exception cref="ValidationException">Thrown when validation fails.</exception>
public async Task<Guid> BookAppointmentAsync(BookAppointmentRequest request)
{
    if (request == null)
        throw new ArgumentNullException(nameof(request));
    
    // Implementation
}
```

### Documenter les APIs

```csharp
/// <summary>
/// Books a new appointment for the authenticated patient.
/// </summary>
/// <param name="request">Appointment booking details.</param>
/// <returns>Newly created appointment details.</returns>
/// <response code="200">Appointment booked successfully.</response>
/// <response code="400">Invalid request data.</response>
/// <response code="401">Unauthorized.</response>
/// <response code="409">Time slot already reserved.</response>
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<IActionResult> BookAppointment(
    [FromBody] BookAppointmentRequest request)
{
    // Implementation
}
```

---

## Pull Request Checklist

Avant de soumettre une PR, assurez-vous que:

- [ ] Le code suit les conventions de style du projet
- [ ] Tous les tests passent: `dotnet test` / `npm test`
- [ ] Le code est documenté et commenté
- [ ] Les problèmes/warnings du linter sont résolus
- [ ] La branche est à jour avec `main`: `git fetch upstream && git rebase upstream/main`
- [ ] Le commit message est clair et descriptif
- [ ] Aucun fichier de configuration sensible n'est committé
- [ ] Les migrations DB sont incluses (si applicable)
- [ ] La documentation a été mise à jour (README, etc.)

### Template de PR

```markdown
## Description
Brief description of what this PR accomplishes.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Linked Issues
Fixes #JIRA-XXX

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Screenshots (if applicable)
[Add screenshots here]

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
```

---

## Questions?

Si vous avez des questions:

1. Consultez la documentation: [docs/](docs/)
2. Ouvrez une issue: [GitHub Issues](https://github.com/your-org/MediLink/issues)
3. Contactez l'équipe: support@medilink.com

---

**Merci de contribuer à MediLink!** 🚀
