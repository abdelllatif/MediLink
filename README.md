# 🏥 MediLink - Plateforme de Gestion du Parcours Médical

## 📋 Vue d'ensemble

**MediLink** est une plateforme hospitalière complète pour la gestion du parcours médical et la téléexpertise médicale. Elle permet:

- ✅ Rationaliser les rendez-vous médicaux
- ✅ Gérer les consultations entre médecins généralistes et spécialistes
- ✅ Tracer les documents médicaux complets
- ✅ Gérer les paiements de consultation
- ✅ Optimiser les créneaux horaires
- ✅ Digitaliser les processus hospitaliers

---

## 🎯 Objectifs Principaux

1. **Rationalisation des services médicaux** : Digitaliser complètement les services hospitaliers
2. **Gestion intelligente du temps** : Optimiser la gestion des créneaux et réduire les temps d'attente
3. **Collaboration médicale** : Faciliter la communication entre généralistes et spécialistes
4. **Traçabilité médicale** : Créer un dossier médical unifié et complet pour chaque patient
5. **Gestion financière** : Automatiser la facturation et les paiements

---

## 👥 Acteurs du Système

### 1. **Infirmier(ère)**
- Réception des patients (avec ou sans dossier)
- Création de nouveau dossier patient
- Enregistrement des signes vitaux (TA, T°, FC)
- Mise en file d'attente

### 2. **Médecin Généraliste**
- Consultation des patients
- Diagnostic et ordonnances
- Demande d'avis spécialisé
- Gestion des créneaux (planning + blocages)
- Prix: **150 DH** par consultation

### 3. **Médecin Spécialiste**
- Réception des demandes de téléexpertise
- Analyse des cas
- Rédaction de rapports médicaux
- Tarifs variables selon le spécialiste

### 4. **Patient**
- Accès sans compte initial possible
- Inscription ultérieure
- Réservation de créneaux
- Paiement initial: **50 DH** (avance)
- Accès aux documents médicaux

### 5. **Administrateur**
- Gestion des utilisateurs
- Configuration du système
- Rapports et analytics

---

## 🏗️ Architecture Générale

```
┌─────────────────────────────────────────────────────┐
│                   Frontend (React)                   │
│         (Vite + Tailwind + React Query)             │
└────────────────────┬────────────────────────────────┘
                     │ HTTP/REST
         ┌───────────┴──────────┐
         │                      │
    ┌────▼──────────┐   ┌──────▼────────┐
    │  SignalR      │   │  ASP.NET Core │
    │  (Real-time)  │   │   Web API     │
    └───────────────┘   └────┬─────┬────┘
                            │     │
         ┌──────────────────┘     └──────────┐
         │                                   │
    ┌────▼────────────┐  ┌─────────────────▼──────┐
    │  Application    │  │  Infrastructure        │
    │  (Services)     │  │  (Data + External)     │
    └──────┬──────────┘  └──────────┬──────────────┘
           │                        │
    ┌──────▼────────────────────────▼──────┐
    │  Domain Layer                         │
    │  (Entities + ValueObjects + Enums)  │
    └──────┬──────────────────────┬────────┘
           │                      │
    ┌──────▼────────┐      ┌─────▼──────────┐
    │  SQL Server   │      │  Services      │
    │  (PostgreSQL) │      │  Externes:     │
    │               │      │  - Stripe      │
    │               │      │  - PDF         │
    │               │      │  - Email       │
    └───────────────┘      └────────────────┘
```

---

## 🛠️ Tech Stack

### **Backend (.NET)**

#### Core Framework
- **ASP.NET Core 8.0** - Web API REST
- **Entity Framework Core 8.0** - ORM
- **C# 12** - Langage principal

#### Architecture
- **Clean Architecture** - Séparation des responsabilités
- **CQRS Pattern** (optional) - Séparation Commandes/Requêtes
- **MediatR** - Mediator pattern
- **Dependency Injection** - Built-in

#### Sécurité & Authentication
- **JWT (JSON Web Tokens)** - Authentification sans état
- **Role-Based Authorization** - Admin, Généraliste, Spécialiste, Infirmier, Patient
- **BCrypt** - Hachage sécurisé des mots de passe

#### Mapping & Validation
- **AutoMapper** - Mapping DTO ↔ Entity
- **FluentValidation** - Validation des modèles

#### Générations de Rapports
- **QuestPDF** - Génération de PDF médicaux et factures

#### Paiements
- **Stripe** - Intégration paiements en ligne
- **Stripe Webhooks** - Notifications en temps réel

#### Real-time & Notifications
- **SignalR** - Communication en temps réel (mises à jour file d'attente, statuts)

#### Logging & Monitoring
- **Serilog** - Logging structuré
- **Serilog.Sinks.File** - Fichiers log
- **Serilog.Sinks.Console** - Console output

#### Documentation API
- **Swagger/OpenAPI** - Documentation interactive

### **Frontend (React)**

#### Build & Runtime
- **React 18.x** - Framework UI
- **Vite** - Build tool (ultra-rapide)
- **Node.js 18+** - Runtime

#### HTTP & State Management
- **Axios** - Client HTTP
- **React Query** - Gestion du cache et requêtes
- **Redux Toolkit** (optionnel) - State global avancé

#### UI & Styling
- **Tailwind CSS** - Utility-first CSS
- **Shadcn/ui** ou **Material UI** - Composants pré-construits

#### Calendrier & Créneaux
- **FullCalendar.js** - Affichage calendrier médical
- Grille personnalisée pour créneaux 30min

#### Paiements Frontend
- **Stripe.js** - Intégration paiements
- **Stripe Checkout** - Redirection paiement sécurisée

#### Upload de Fichiers
- **React Dropzone** - Upload fichiers médicaux
- Stockage local (dev) / Cloud (production)

#### Dev Tools
- **React DevTools** - Debugging composants
- **Vite DevTools** - Debugging build
- **Postman** - Testing API

### **Base de Données**

#### Option 1: SQL Server (Recommandée)
```
SQL Server 2019+ ou Azure SQL Database
Support complet EF Core
Haute disponibilité
```

#### Option 2: PostgreSQL
```
PostgreSQL 13+
EF Core adapté
Open-source
Moins de coûts
```

### **Infrastructure & Deployment**

- **Docker** - Containerisation (optionnel)
- **Azure App Service** ou **IIS** - Hosting backend
- **Azure Static Web Apps** ou **Vercel** - Hosting frontend
- **Azure SQL Database** - Base de données cloud
- **GitHub Actions** - CI/CD

---

## 📁 Structure du Projet

```
MediLink/
│
├── MediLink.sln
│
├── README.md (ce fichier)
├── docs/
│   ├── CAHIER_DES_CHARGES.md
│   ├── ARCHITECTURE.md
│   ├── API_DOCUMENTATION.md
│   └── BUSINESS_RULES.md
│
├── src/
│   │
│   ├── MediLink.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── PatientsController.cs
│   │   │   ├── AppointmentsController.cs
│   │   │   ├── DoctorsController.cs
│   │   │   ├── SpecialistsController.cs
│   │   │   ├── PaymentsController.cs
│   │   │   ├── MedicalDocumentsController.cs
│   │   │   └── TimeSlotsController.cs
│   │   │
│   │   ├── Middlewares/
│   │   │   ├── ExceptionMiddleware.cs
│   │   │   └── JwtMiddleware.cs
│   │   │
│   │   ├── Extensions/
│   │   │   ├── ServiceExtensions.cs
│   │   │   ├── SwaggerExtensions.cs
│   │   │   └── AuthExtensions.cs
│   │   │
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── MediLink.API.csproj
│   │
│   ├── MediLink.Application/
│   │   ├── DTOs/
│   │   │   ├── PatientDto.cs
│   │   │   ├── DoctorDto.cs
│   │   │   ├── AppointmentDto.cs
│   │   │   ├── SpecialistDto.cs
│   │   │   ├── PaymentDto.cs
│   │   │   └── MedicalDocumentDto.cs
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── IPatientService.cs
│   │   │   ├── IAppointmentService.cs
│   │   │   ├── IDoctorService.cs
│   │   │   ├── IPaymentService.cs
│   │   │   └── IMedicalDocumentService.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── PatientService.cs
│   │   │   ├── AppointmentService.cs
│   │   │   ├── DoctorService.cs
│   │   │   ├── PaymentService.cs
│   │   │   └── MedicalDocumentService.cs
│   │   │
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   │
│   │   └── MediLink.Application.csproj
│   │
│   ├── MediLink.Domain/
│   │   ├── Entities/
│   │   │   ├── Patient.cs
│   │   │   ├── Doctor.cs
│   │   │   ├── Specialist.cs
│   │   │   ├── Nurse.cs
│   │   │   ├── Appointment.cs
│   │   │   ├── MedicalDocument.cs
│   │   │   ├── Payment.cs
│   │   │   ├── TimeSlot.cs
│   │   │   ├── MedicalAct.cs
│   │   │   └── User.cs
│   │   │
│   │   ├── Enums/
│   │   │   ├── AppointmentStatus.cs
│   │   │   ├── PaymentStatus.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── TimeSlotStatus.cs
│   │   │   ├── MedicalActType.cs
│   │   │   └── PriorityLevel.cs
│   │   │
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs
│   │   │   ├── Address.cs
│   │   │   └── VitalSigns.cs
│   │   │
│   │   └── MediLink.Domain.csproj
│   │
│   ├── MediLink.Infrastructure/
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── PatientConfig.cs
│   │   │   │   ├── AppointmentConfig.cs
│   │   │   │   ├── DoctorConfig.cs
│   │   │   │   └── TimeSlotConfig.cs
│   │   │   │
│   │   │   └── Migrations/
│   │   │       └── (EF Core migrations)
│   │   │
│   │   ├── Repositories/
│   │   │   ├── PatientRepository.cs
│   │   │   ├── AppointmentRepository.cs
│   │   │   ├── DoctorRepository.cs
│   │   │   ├── TimeSlotRepository.cs
│   │   │   └── BaseRepository.cs
│   │   │
│   │   ├── ExternalServices/
│   │   │   ├── StripeService.cs
│   │   │   ├── PdfService.cs
│   │   │   └── EmailService.cs
│   │   │
│   │   ├── DependencyInjection.cs
│   │   └── MediLink.Infrastructure.csproj
│   │
│   └── MediLink.Shared/
│       ├── Constants/
│       │   ├── Roles.cs
│       │   ├── Prices.cs
│       │   └── AppConstants.cs
│       │
│       ├── Helpers/
│       │   ├── DateHelper.cs
│       │   └── CalculationHelper.cs
│       │
│       └── MediLink.Shared.csproj
│
├── tests/
│   ├── MediLink.UnitTests/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── MediLink.UnitTests.csproj
│   │
│   └── MediLink.IntegrationTests/
│       ├── Controllers/
│       ├── Services/
│       └── MediLink.IntegrationTests.csproj
│
├── frontend/
│   ├── src/
│   │   ├── pages/
│   │   │   ├── Login/
│   │   │   ├── Dashboard/
│   │   │   ├── Patients/
│   │   │   ├── Doctors/
│   │   │   ├── Appointments/
│   │   │   ├── TimeSlots/
│   │   │   ├── Payments/
│   │   │   ├── MedicalDocuments/
│   │   │   └── AdminPanel/
│   │   │
│   │   ├── components/
│   │   │   ├── Common/
│   │   │   ├── Calendar/
│   │   │   ├── Forms/
│   │   │   └── Layout/
│   │   │
│   │   ├── services/
│   │   │   ├── api.ts
│   │   │   ├── authService.ts
│   │   │   ├── appointmentService.ts
│   │   │   └── ...
│   │   │
│   │   ├── hooks/
│   │   │   ├── useAuth.ts
│   │   │   ├── useAppointments.ts
│   │   │   └── ...
│   │   │
│   │   ├── context/
│   │   │   ├── AuthContext.tsx
│   │   │   └── ...
│   │   │
│   │   ├── utils/
│   │   │   ├── formatters.ts
│   │   │   ├── validators.ts
│   │   │   └── ...
│   │   │
│   │   ├── App.tsx
│   │   ├── main.tsx
│   │   └── styles/
│   │       └── globals.css
│   │
│   ├── public/
│   ├── package.json
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── tailwind.config.js
│   └── postcss.config.js
│
├── .gitignore
├── .env.example
├── docker-compose.yml
├── .github/
│   └── workflows/
│       ├── backend-ci.yml
│       └── frontend-ci.yml
│
└── (Fichiers de configuration)
```

---

## 💾 Entités Principales

### **1. Patient**
```csharp
- Id (Guid)
- Nom, Prénom
- Email, Téléphone
- CIN (optionnel)
- DateNaissance
- Adresse (ValueObject)
- Consultations (Navigation)
- Appointments (Navigation)
- MedicalDocuments (Navigation)
```

### **2. Médecin (Doctor)**
```csharp
- Id (Guid)
- Nom, Prénom
- Spécialité
- Cabinet
- Expérience
- Créneaux (Navigation)
- Consultations (Navigation)
- BlockedDays (Navigation)
```

### **3. Créneau (TimeSlot)**
```csharp
- Id (Guid)
- DoctorId
- Date
- Heure (30 min)
- Status (LIBRE, RÉSERVÉ, BLOQUÉ)
- AppointmentId (optionnel)
- Tarif (Money)
```

### **4. Rendez-vous (Appointment)**
```csharp
- Id (Guid)
- PatientId
- DoctorId
- TimeSlotId
- Status (SCHEDULED, COMPLETED, CANCELLED)
- Montant (Money)
- DateCreation
```

### **5. Document Médical (MedicalDocument)**
```csharp
- Id (Guid)
- PatientId
- DoctorId
- SpecialistId (optionnel)
- Diagnostic
- Prescription
- MedicalActs (Navigation)
- Total (Money)
- DateCreation
```

### **6. Acte Médical (MedicalAct)**
```csharp
- Id (Guid)
- Type (Enum: IRM, Radio, ECG, etc.)
- Prix (Money)
- MedicalDocumentId
```

### **7. Paiement (Payment)**
```csharp
- Id (Guid)
- AppointmentId
- MedicalDocumentId
- Montant (Money)
- Status (PENDING, COMPLETED, FAILED)
- StripeTransactionId
- DatePaiement
```

---

## 📊 Règles Métier Clés

### **Créneaux (Time Slots)**
- ✅ Durée: **30 minutes**
- ✅ Horaires: 09:00 - 17:00
- ✅ Jours travail: Lundi - Vendredi
- ✅ Weekend: **Samedi/Dimanche fermés**
- ✅ Affichage: Jour + 6 jours suivants (excl. weekend)

### **Statuts Créneaux**
```
🟢 LIBRE       → Disponible à la réservation
🔴 RÉSERVÉ     → Occupé par un patient
⚫ BLOQUÉ       → Indisponible (congés, conférence)
```

### **Réservation (Booking)**
```
Flux:
1. Patient choisit créneau
2. Vérification disponibilité
3. Paiement avance: 50 DH
4. Confirmation rendez-vous
5. MedicalDocument créé
```

### **Tarification**
```
Consultation Généraliste     = 150 DH
Avance Réservation           = 50 DH
Consultation Spécialiste     = Variable (selon tarif du spécialiste)
Actes Médicaux              = À la carte

Total = Consultation + Expertise + Actes Médicaux
```

### **Patient sans Compte**
```
✅ Admission directe possible
✅ Enregistré par infirmier
❌ Pas de réservation créneau initialement
✅ Peut créer un compte ultérieurement
```

---

## 🔐 Authentification & Autorisation

### **Rôles Utilisateurs**
```
- ADMIN           → Gestion complète du système
- GENERALIST      → Médecin généraliste
- SPECIALIST      → Médecin spécialiste
- NURSE           → Infirmier(ère)
- PATIENT         → Patient
```

### **JWT Token Claims**
```
{
  "sub": "userId",
  "email": "user@medilink.com",
  "role": "GENERALIST",
  "exp": 1704067200,
  "iat": 1704067200
}
```

---

## 📡 Flux Real-time (SignalR)

### **Cas d'Usage**
```
1. Mise à jour file d'attente en temps réel
2. Notification nouveau rendez-vous
3. Changement statut appointment
4. Notification paiement reçu
```

### **Hub SignalR**
```csharp
AppointmentHub
├── JoinAppointmentGroup(appointmentId)
├── LeaveAppointmentGroup(appointmentId)
├── BroadcastAppointmentUpdate(appointment)
└── NotifyQueueUpdate(patients)

PaymentHub
├── NotifyPaymentSuccess(payment)
└── NotifyPaymentFailed(payment)
```

---

## 💳 Intégration Stripe

### **Flux Paiement**
```
1. React → Créer session Checkout
2. API → Stripe API (create checkout session)
3. Redirection → Stripe Checkout
4. Client → Paiement sécurisé
5. Webhook → API reçoit confirmation
6. API → Update status payment
7. SignalR → Notifier clients
```

### **Endpoints Stripe**
```
POST /api/payments/create-checkout-session
POST /api/webhooks/stripe (webhook endpoint)
GET  /api/payments/{id}
```

---

## 📄 Génération PDF (QuestPDF)

### **Documents Générés**
```
1. Rapport Médical
   ├── Patient info
   ├── Diagnostic
   ├── Prescription
   └── Médecin signature

2. Facture/Invoice
   ├── Services rendus
   ├── Montants détaillés
   ├── Total à payer
   └── QR code paiement

3. Ordonnance
   ├── Médicaments prescrit
   ├── Posologie
   └── Durée traitement
```

---

## 🚀 Getting Started

### **Prérequis Backend**
```bash
- .NET SDK 8.0+
- SQL Server 2019+ ou PostgreSQL 13+
- Visual Studio 2022 ou VS Code
```

### **Prérequis Frontend**
```bash
- Node.js 18+
- npm ou yarn
- VS Code
```

### **Installation Backend**
```bash
# Cloner le repo
git clone https://github.com/your-org/MediLink.git
cd MediLink

# Restaurer dépendances
dotnet restore

# Configuration BD
dotnet ef migrations add InitialCreate --project src/MediLink.Infrastructure
dotnet ef database update --project src/MediLink.Infrastructure

# Démarrer API
cd src/MediLink.API
dotnet run
# API disponible: http://localhost:5000
```

### **Installation Frontend**
```bash
cd frontend

# Installer dépendances
npm install

# Démarrer dev server
npm run dev
# Frontend disponible: http://localhost:5173
```

---

## 🧪 Testing

### **Unit Tests**
```bash
cd tests/MediLink.UnitTests
dotnet test
```

### **Integration Tests**
```bash
cd tests/MediLink.IntegrationTests
dotnet test
```

---

## 📚 Documentation Additionnelle

- [📖 CAHIER_DES_CHARGES.md](docs/CAHIER_DES_CHARGES.md)
- [🏛️ ARCHITECTURE.md](docs/ARCHITECTURE.md)
- [📡 API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md)
- [📋 BUSINESS_RULES.md](docs/BUSINESS_RULES.md)

---

## 🔄 CI/CD Pipeline

### **GitHub Actions**
```yaml
✅ Backend Tests (xUnit)
✅ Frontend Tests (Vitest)
✅ SonarQube Analysis
✅ Docker Build
✅ Deploy to Azure
```

---

## 📝 Git Workflow

```bash
# Feature branches
git checkout -b feature/JIRA-XXX-description

# Commit messages
git commit -m "feat(JIRA-XXX): Add patient appointment feature"

# Push & Pull Request
git push origin feature/JIRA-XXX-description
```

---

## 🤝 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/amazing-feature`)
3. Commit vos changements (`git commit -m 'Add amazing feature'`)
4. Push vers la branche (`git push origin feature/amazing-feature`)
5. Ouvrir une Pull Request

---

## 📄 License

Ce projet est sous license MIT - voir le fichier `LICENSE` pour plus de détails.

---

## 👨‍💼 Auteur

**Abdellatif Hissoune** - Architecte et Lead Developer

---

## 📞 Support

Pour toute question ou support, contactez: support@medilink.com

---

**MediLink** © 2024-2025 - Tous droits réservés
