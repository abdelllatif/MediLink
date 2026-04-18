# 📋 CAHIER DES CHARGES - MediLink

## Document d'Exigences Fonctionnelles et Techniques

**Version:** 1.0  
**Date:** 2024  
**Statut:** Approuvé  

---

## TABLE DES MATIÈRES

1. [Contexte du Projet](#contexte)
2. [Objectifs](#objectifs)
3. [Acteurs et Cas d'Usage](#acteurs)
4. [Exigences Fonctionnelles](#exigences-fonctionnelles)
5. [Exigences Techniques](#exigences-techniques)
6. [Exigences de Sécurité](#exigences-securite)
7. [Exigences de Performance](#exigences-performance)
8. [Règles Métier](#regles-metier)
9. [Données et Stockage](#donnees)
10. [Intégrations Externes](#integrations)

---

## 1. CONTEXTE DU PROJET {#contexte}

### 1.1 Description Générale

**MediLink** est une plateforme numérique complète pour la gestion du parcours médical dans une structure hospitalière. Le système doit permettre:

- La gestion des patients
- La réservation de rendez-vous
- La gestion des consultations
- La traçabilité des documents médicaux
- La gestion des paiements
- La téléexpertise médicale

### 1.2 Contexte Métier

La plateforme doit se substituer aux systèmes manuels ou partiellement numérisés pour:

- **Réduire les temps d'attente** grâce à une meilleure gestion des créneaux
- **Améliorer la collaboration** entre généralistes et spécialistes
- **Garantir la traçabilité** complète du parcours médical
- **Automatiser la facturation** et les paiements
- **Optimiser l'utilisation** des ressources médicales

### 1.3 Portée du Projet

**Phase 1 (MVP):**
- ✅ Module Patient
- ✅ Module Infirmier
- ✅ Module Médecin Généraliste
- ✅ Module Réservation
- ✅ Module Paiement basique

**Phase 2 (Évolutions):**
- 🔄 Module Spécialiste avancé
- 🔄 Téléexpertise
- 🔄 Vidéoconsultation (SignalR)

---

## 2. OBJECTIFS {#objectifs}

### 2.1 Objectifs Métier

| Objectif | Description | Indicateur |
|----------|-------------|-----------|
| **Réduction délais** | Diminuer temps attente | -60% vs actuel |
| **Satisfaction patients** | Meilleure expérience | +80% NPS |
| **Efficacité médecins** | Gain de temps | +30% capacité |
| **Traçabilité** | Audit complet | 100% tracé |
| **Paiements** | Automatisation | 95% digital |

### 2.2 Objectifs Techniques

- Scalabilité jusqu'à 10,000 utilisateurs concurrents
- API réactive (< 200ms)
- Disponibilité: 99.9% uptime
- Sécurité: Conformité HIPAA (données sensibles)
- Architecture maintenable et évolutive

---

## 3. ACTEURS ET CAS D'USAGE {#acteurs}

### 3.1 Infirmier

#### US1.1: Accueil Patient

**Acteur:** Infirmier  
**Déclencheur:** Arrivée patient  
**Prérequis:** Patient physiquement à l'hôpital

**Flux Principal:**
```
1. Infirmier accède au module "Accueil"
2. Recherche patient par Nom/CIN/Téléphone
3. Si trouvé:
   - Affiche le dossier existant
   - Mise à jour signes vitaux
4. Si non trouvé:
   - Création nouveau dossier
   - Saisie données de base
5. Enregistrement signes vitaux:
   - Tension artérielle
   - Température
   - Fréquence cardiaque
6. Mise en file d'attente
```

**Données Saisies:**
```
Si Nouveau Patient:
- Nom, Prénom
- Âge / Date de naissance
- Téléphone
- CIN (optionnel)

Signes Vitaux:
- TA (ex: 120/80)
- Température (°C)
- FC (bpm)
- Poids (kg)
```

**Postcondition:**
- Patient en file d'attente
- Document médical créé ou mis à jour
- Historique tracé

---

#### US1.2: Gestion File d'Attente

**Acteur:** Infirmier  
**Description:** Vue globale des patients en attente

**Fonctionnalités:**
```
✅ Liste des patients en attente
✅ Tri par ordre d'arrivée
✅ Affichage signes vitaux
✅ Durée d'attente calculée
✅ Appel patient suivant
✅ Transfert vers médecin
```

---

### 3.2 Médecin Généraliste

#### US2.1: Consultation Patient

**Acteur:** Médecin Généraliste  
**Déclencheur:** Patient en file d'attente

**Flux:**
```
1. Médecin sélectionne patient
2. Affiche dossier médical complet
3. Saisit consultation:
   - Symptômes
   - Diagnostic
   - Traitement/Prescription
4. Crée un document médical
5. Calcule montant consultation (150 DH)
6. Valide consultation
```

**Prix:** 150 DH TTC

**Document Généré:**
- Diagnostic
- Prescription
- Ordonnance (PDF)

---

#### US2.2: Demande Avis Spécialiste

**Acteur:** Médecin Généraliste  
**Prérequis:** Consultation créée

**Flux:**
```
1. Médecin choisit spécialité requise
2. Sélectionne spécialiste disponible
3. Choisit niveau priorité:
   - URGENT
   - NORMAL
   - NON-URGENT
4. Joint dossier médical
5. Envoie demande
```

**Statut Demande:**
```
PENDING       → En attente
IN_PROGRESS   → En analyse
COMPLETED     → Réponse reçue
```

---

#### US2.3: Gestion Créneaux

**Acteur:** Médecin Généraliste

##### A) Consultation des Créneaux

```
✅ Vue calendrier (7 jours)
✅ Affichage par jour
✅ Créneaux 30 minutes
✅ Code couleur:
   - Vert: Libre
   - Rouge: Réservé
   - Noir: Bloqué
```

**Planning Visible:**
```
Aujourd'hui (Today)
Jour 1
Jour 2
Jour 3
Jour 4
Jour 5 (max)

❌ Weekend exclus
❌ Jours fériés exclus
```

**Horaires:**
```
09:00 - 09:30
09:30 - 10:00
10:00 - 10:30
...
17:00 - 17:30
```

---

##### B) Blocage Jours

**Acteur:** Médecin Généraliste

**Cas d'Usage:** Bloquer journée entière

```
1. Médecin accède à "Mes créneaux"
2. Sélectionne jour à bloquer
3. Raison (optionnel):
   - Congé
   - Conférence
   - Indisponibilité
4. Confirme blocage

Résultat:
- Tous créneaux du jour = BLOQUÉ
- Non-réservable par patients
```

---

### 3.3 Médecin Spécialiste

#### US3.1: Réception Demandes

**Acteur:** Médecin Spécialiste

```
1. Reçoit notifications demandes
2. Affiche liste demandes en attente
3. Tri par priorité:
   - URGENT (rouge)
   - NORMAL (orange)
   - NON-URGENT (vert)
4. Consulte dossier patient
5. Analyse cas
```

---

#### US3.2: Réponse Avis

**Acteur:** Médecin Spécialiste

```
1. Rédige avis médical
2. Ajoute recommandations
3. Dossier généré (PDF)
4. Valide et envoie

Résultat:
- Avis transmis à généraliste
- Notification envoyée
- Document archivé
```

---

### 3.4 Patient

#### US4.1: Réservation Créneau

**Acteur:** Patient  
**Prérequis:** Compte créé OU Enregistrement rapide

**Flux:**
```
1. Patient accède "Réserver rendez-vous"
2. Choisit médecin:
   - Liste des généralistes
   - Affichage spécialité
   - Filtres (localisation, avis)
3. Consulte disponibilités:
   - Calendrier interactif
   - Créneaux LIBRES uniquement
4. Choisit créneau
5. Paiement avance (50 DH):
   - Redirection Stripe
   - Sécurisé (PCI compliant)
6. Confirmation rendez-vous:
   - Email confirmation
   - Détails rendez-vous
   - Rappel 24h avant
```

**Montant Avance:** 50 DH

**Post-condition:**
- Rendez-vous créé (SCHEDULED)
- Créneau = RÉSERVÉ
- Invoice générée
- Confirmation envoyée

---

#### US4.2: Gestion Profil

**Acteur:** Patient

```
✅ Affichage informations personnelles
✅ Modification données
✅ Historique rendez-vous
✅ Accès documents médicaux
✅ Factures/Paiements
```

---

### 3.5 Administrateur

#### US5.1: Gestion Utilisateurs

**Acteur:** Admin

```
✅ Créer/Modifier/Supprimer utilisateurs
✅ Attribution rôles
✅ Réinitialisation mots de passe
✅ Audit logs
```

---

## 4. EXIGENCES FONCTIONNELLES {#exigences-fonctionnelles}

### 4.1 Authentification & Autorisation

#### REQ-AUTH-001: JWT Authentication
```
✅ Token JWT avec expiration 1 heure
✅ Refresh token (7 jours)
✅ Hachage mot de passe BCrypt
✅ Role-based access control (RBAC)
✅ Rôles: ADMIN, GENERALIST, SPECIALIST, NURSE, PATIENT
```

#### REQ-AUTH-002: Contrôle d'Accès
```
✅ Chaque endpoint protégé par rôle
✅ Données filtrées par utilisateur
✅ Audit trail des accès sensibles
```

---

### 4.2 Module Patient

#### REQ-PAT-001: Création Patient
```
✅ Avec compte (inscription)
✅ Sans compte (accueil infirmier)
✅ Enregistrement minimal: Nom, Prénom, Téléphone
✅ Optionnel: CIN, Email
✅ Validation CIN marocain (format)
```

#### REQ-PAT-002: Dossier Médical Unifié
```
✅ Profil patient complet
✅ Historique consultations
✅ Documents médicaux archivés
✅ Prescriptions
✅ Paiements
✅ Rendez-vous futurs
```

#### REQ-PAT-003: Recherche Patient
```
✅ Par nom + prénom
✅ Par téléphone
✅ Par CIN
✅ Recherche fuzzy (tolère petites fautes)
```

---

### 4.3 Module Créneau (TimeSlot)

#### REQ-SLOT-001: Création Créneaux
```
✅ Génération automatique (30 min)
✅ 09:00 - 17:30
✅ Lun - Ven uniquement
✅ N créneaux par médecin par jour
✅ Tarif configurable par créneau
```

#### REQ-SLOT-002: Gestion Statuts
```
✅ LIBRE     → Réservable
✅ RÉSERVÉ   → Occupé
✅ BLOQUÉ    → Indisponible
✅ Impossible de passer de RÉSERVÉ → LIBRE
```

#### REQ-SLOT-003: Blocage Journées
```
✅ Médecin peut bloquer jour entier
✅ Tous créneaux = BLOQUÉ
✅ Raison optionnelle
✅ Déblocage possible
```

---

### 4.4 Module Rendez-vous

#### REQ-APT-001: Création Rendez-vous
```
✅ Via réservation créneau (patient)
✅ Manuel (admin)
✅ Statuts: SCHEDULED, COMPLETED, CANCELLED, PENDING_PAYMENT
```

#### REQ-APT-002: Cycle Vie Rendez-vous
```
SCHEDULED
    ↓
COMPLETED (après consultation)
    ↓
PAYMENT_PROCESSED
    ↓
ARCHIVED

Ou:

SCHEDULED → CANCELLED (patient ou médecin)
        → PENDING_PAYMENT → PAYMENT_FAILED
```

#### REQ-APT-003: Reminders
```
✅ Email 24h avant
✅ SMS optionnel (phase 2)
✅ Notification app
```

---

### 4.5 Module Consultation

#### REQ-CONS-001: Enregistrement
```
✅ Saisie symptômes (text)
✅ Diagnostic (text)
✅ Prescription (text)
✅ Actes médicaux additionnels (liste)
✅ Signature numérique médecin
✅ Timestamp automatique
```

#### REQ-CONS-002: Tarification
```
Montant = 150 DH (consultation base)
        + Prix actes médicaux
        + Prix expertise (si spécialiste)

Exemple:
- Consultation généraliste: 150 DH
- Radiographie: 300 DH
- Analyse lab: 200 DH
- Total: 650 DH
- Patient payé avance: 50 DH
- Solde à payer: 600 DH
```

---

### 4.6 Module Document Médical

#### REQ-DOC-001: Génération Automatique
```
✅ PDF généré après consultation
✅ QuestPDF pour la mise en forme
✅ Données complètes du patient
✅ Diagnostic, Prescription
✅ Signature numérique
✅ Archivé dans système
```

#### REQ-DOC-002: Contenu Document
```
En-tête:
  - Logo hôpital
  - Références (N° consultation)
  
Patient:
  - Nom, Prénom
  - CIN, Âge
  - Téléphone
  
Médecin:
  - Nom, Prénom
  - Spécialité
  
Corps:
  - Motif consultation
  - Diagnostic
  - Prescription
  - Actes recommandés
  
Pied:
  - Date signature
  - Signature numérique
  - QR code dossier
```

---

### 4.7 Module Paiement

#### REQ-PAY-001: Intégration Stripe
```
✅ Créer session checkout
✅ Redirection vers Stripe
✅ Capture paiement sécurisé
✅ Webhook confirmation
✅ Mise à jour statut
```

#### REQ-PAY-002: Montants
```
Phase 1 (Réservation):
  - Avance 50 DH (non-remboursable)
  
Phase 2 (Consultation):
  - Solde consultation
  - Actes supplémentaires
  
Options:
  - Paiement complet à la réservation
  - Paiement partiel + solde au jour J
```

#### REQ-PAY-003: Invoice
```
✅ Générée automatiquement
✅ Numérotation séquentielle
✅ Détails services
✅ Montants bruts/nets
✅ Téléchargeable (PDF)
✅ Archivée 7 ans (légal)
```

---

### 4.8 Module Real-time (SignalR)

#### REQ-RT-001: Notifications
```
✅ Nouvelle demande spécialiste → spécialiste
✅ Nouveau rendez-vous → médecin
✅ Paiement reçu → admin
✅ File d'attente mise à jour → infirmier
```

#### REQ-RT-002: Update File Attente
```
✅ Broadcast à tous infirmiers
✅ Affichage patient suivant
✅ Durée d'attente live
✅ Appel patient réel-time
```

---

## 5. EXIGENCES TECHNIQUES {#exigences-techniques}

### 5.1 Backend

#### Framework & Langage
```
- .NET 8 LTS (C# 12)
- ASP.NET Core 8
- Entity Framework Core 8
- Microsoft.AspNetCore.SignalR
```

#### Dépendances Principales
```
AutoMapper (mapping DTO/Entity)
FluentValidation (validation)
MediatR (mediator pattern)
Serilog (logging)
Stripe.net (paiement)
QuestPDF (génération PDF)
JWT Bearer (authentification)
Swagger/Swashbuckle (API doc)
```

#### Architecture
```
Clean Architecture 4 couches:
- API Layer (Controllers)
- Application Layer (Services)
- Domain Layer (Entities)
- Infrastructure Layer (Data, External)

SOLID Principles:
- Single Responsibility
- Open/Closed
- Liskov Substitution
- Interface Segregation
- Dependency Inversion
```

---

### 5.2 Frontend

#### Framework & Outils
```
- React 18.x
- TypeScript 5.x
- Vite (build tool)
- Tailwind CSS
- React Query
- Axios
- React Router v6
```

#### Dépendances Principales
```
FullCalendar.js (calendrier)
date-fns (manipulation dates)
Stripe.js (paiement)
React Hook Form (formulaires)
Zod (validation schémas)
Shadcn/ui (composants UI)
Framer Motion (animations)
```

---

### 5.3 Base de Données

#### Option Recommandée: SQL Server
```
- SQL Server 2019+ or Azure SQL Database
- EF Core 8 provider natif
- Haute disponibilité
- Backup automatisé
```

#### Option Alternative: PostgreSQL
```
- PostgreSQL 13+
- Npgsql (EF Core provider)
- Open-source
- Moins de coûts
```

#### Schéma Données
```
Tables Principales:
- Users (patients, médecins, etc.)
- Patients
- Doctors
- Specialists
- TimeSlots
- Appointments
- MedicalDocuments
- MedicalActs
- Payments
- BlockedDays
- Consultations

Tables Audit:
- AuditLogs
- PaymentLogs
- LoginAttempts
```

---

## 6. EXIGENCES DE SÉCURITÉ {#exigences-securite}

### 6.1 Authentification

```
REQ-SEC-001: Mots de passe
✅ Min 8 caractères
✅ 1 majuscule, 1 minuscule, 1 chiffre, 1 spécial
✅ Hachage BCrypt (salt rounds: 12)
✅ Jamais stocké en clair

REQ-SEC-002: JWT Tokens
✅ Expiration 1 heure
✅ Refresh token 7 jours
✅ Signature HS256 / RS256
✅ Inclure rôle, userId, email

REQ-SEC-003: 2FA (Phase 2)
✅ OTP via Email/SMS
```

---

### 6.2 Données Sensibles

```
REQ-SEC-004: PII (Personally Identifiable Info)
✅ Chiffrement données sensibles (AES-256)
✅ CIN, N° Sécurité Sociale, etc.
✅ Audit trail complet

REQ-SEC-005: HIPAA Compliance
✅ Accès restreint données patients
✅ Audit logs immuables
✅ Chiffrement en transit (HTTPS)
✅ Chiffrement au repos

REQ-SEC-006: GDPR (si EU)
✅ Droit à l'oubli
✅ Export données
✅ Consentement explicite
```

---

### 6.3 API Security

```
REQ-SEC-007: Endpoints
✅ HTTPS obligatoire
✅ Rate limiting (100 req/min par IP)
✅ CORS configuré
✅ Input validation (XSS, SQL injection)

REQ-SEC-008: CORS
✅ Domaines whitelist
✅ Méthodes autorisées: GET, POST, PUT, DELETE
✅ Headers: Content-Type, Authorization

REQ-SEC-009: SQL Injection Prevention
✅ Parameterized queries (EF Core)
✅ ORM abstraction
✅ Input validation
```

---

### 6.4 Stripe Integration

```
REQ-SEC-010: PCI DSS
✅ Jamais traiter données CB directement
✅ Utiliser Stripe Hosted Checkout
✅ Webhook signé + validé
✅ Secrets en variables d'environnement

REQ-SEC-011: Stripe Webhooks
✅ Signature vérifiée (X-Stripe-Signature)
✅ Idempotence (re-treat paiements)
✅ Timeout handling
```

---

## 7. EXIGENCES DE PERFORMANCE {#exigences-performance}

### 7.1 Backend

```
REQ-PERF-001: API Response Times
✅ GET requests: < 200ms
✅ POST/PUT requests: < 500ms
✅ Queries complexes: < 1s
✅ Under 100 concurrent users

REQ-PERF-002: Database
✅ Indexes sur colonnes fréquemment filtrées
✅ Query optimization
✅ Connection pooling
✅ Max 1000 concurrent connections

REQ-PERF-003: Caching
✅ Redis (optionnel)
✅ Cache consultations (30 min)
✅ Cache créneaux (5 min)
✅ Cache doctors directory (1h)
```

---

### 7.2 Frontend

```
REQ-PERF-004: Page Load
✅ First Contentful Paint: < 1s
✅ Largest Contentful Paint: < 2.5s
✅ Cumulative Layout Shift: < 0.1

REQ-PERF-005: Bundle Size
✅ Initial JS: < 200KB (gzipped)
✅ React Query cache: 10MB max
✅ Lazy loading images
✅ Code splitting par route

REQ-PERF-006: Real-time Limits
✅ Max 10 SignalR connections/user
✅ Message queue: 100 msg/s
✅ Broadcast latency: < 100ms
```

---

## 8. RÈGLES MÉTIER {#regles-metier}

### 8.1 Patients

```
RULE-PAT-001: Accès Sans Compte
✅ Patient peut arriver sans compte
✅ Infirmier crée profil minimal
✅ Email optionnel initialement
❌ Réservation créneau nécessite email

RULE-PAT-002: Création Compte Automatique
✅ Lors 1ère réservation
✅ Email invitation confirmation
✅ Confirmation email = compte actif

RULE-PAT-003: Données Obligatoires
✅ Nom, Prénom
✅ Téléphone (format: +212XXXXXXXXX)
✅ Email (pour réservation)
✅ CIN optionnel
```

---

### 8.2 Créneaux & Réservations

```
RULE-SLOT-001: Durée Créneaux
✅ Exactement 30 minutes
✅ 09:00 - 17:30
✅ Dimanche - Samedi: 2 jours/semaine

RULE-SLOT-002: Affichage Disponibilités
✅ Jour actuel + 6 jours futurs
✅ Excluant weekends automatiquement
✅ Exemple lundi: Mon, Tue, Wed, Thu, Fri, Mon

RULE-SLOT-003: Réservation
✅ Créneau LIBRE seulement
✅ Patient non-déjà réservé même jour
✅ Avance 50 DH non-remboursable
✅ Confirmation immédiate

RULE-SLOT-004: Annulation
✅ Patient: jusqu'à 24h avant (-50 DH pénalité)
✅ Médecin: déblocage possible
✅ Raison tracée
```

---

### 8.3 Consultations

```
RULE-CONS-001: Tarification
Montant = Consultation Base (150 DH)
        + Actes Médicaux (à la carte)
        + Expertise Spécialiste (variable)

RULE-CONS-002: Facturation
✅ Invoice créée à fin consultation
✅ Patient payé avance: 50 DH
✅ Solde: Total - Avance
✅ Délai paiement: 24h ou immédiat

RULE-CONS-003: Documents
✅ 1 Document Médical par Consultation
✅ PDF généré automatiquement
✅ Archivé légalement (7 ans)
```

---

### 8.4 Blocages

```
RULE-BLOCK-001: Blocage Jour
✅ Médecin bloque entière journée
✅ Raison: Congé, Conférence, Indisponibilité
✅ Tous créneaux = BLOQUÉ
✅ Patients notifiés si réservation existante

RULE-BLOCK-002: Déblocage
✅ Médecin peut débloquer
✅ Créneau redevient LIBRE si non-réservé
```

---

### 8.5 Paiements

```
RULE-PAY-001: Avance Réservation
✅ 50 DH requis pour réserver
✅ Non-remboursable (sauf raison valable)
✅ Stripe traite le paiement
✅ Immédiat

RULE-PAY-002: Solde Consultation
✅ Après consultation
✅ Calcul automatique (Total - Avance)
✅ Délai paiement: 24-72h max
✅ Rappel email

RULE-PAY-003: Historique
✅ Invoice archivée 7 ans (légal)
✅ Audit trail des paiements
✅ Traçabilité complète
```

---

## 9. DONNÉES ET STOCKAGE {#donnees}

### 9.1 Schéma Simplifié

```sql
-- Users (héritage polymorphe)
Users
  ├── Id (Guid, PK)
  ├── Email (string, unique)
  ├── PasswordHash (string)
  ├── Role (enum: ADMIN, GENERALIST, SPECIALIST, NURSE, PATIENT)
  ├── IsActive (bool)
  ├── CreatedAt (datetime)
  └── UpdatedAt (datetime)

-- Patients
Patients
  ├── Id (Guid, PK)
  ├── UserId (Guid, FK)
  ├── FirstName (string)
  ├── LastName (string)
  ├── Phone (string)
  ├── CIN (string, optional)
  ├── DateOfBirth (datetime)
  ├── Address (ValueObject)
  └── Consultations (ICollection)

-- Doctors (Généralistes)
Doctors
  ├── Id (Guid, PK)
  ├── UserId (Guid, FK)
  ├── FirstName (string)
  ├── LastName (string)
  ├── Specialization (string)
  ├── Cabinet (string)
  ├── ExperienceYears (int)
  ├── MaxPatientsPerDay (int)
  ├── TimeSlots (ICollection)
  └── BlockedDays (ICollection)

-- TimeSlots
TimeSlots
  ├── Id (Guid, PK)
  ├── DoctorId (Guid, FK)
  ├── Date (DateTime)
  ├── StartTime (TimeSpan)
  ├── EndTime (TimeSpan)
  ├── Status (enum: LIBRE, RÉSERVÉ, BLOQUÉ)
  ├── Price (Money - ValueObject)
  └── AppointmentId (Guid?, FK)

-- Appointments
Appointments
  ├── Id (Guid, PK)
  ├── PatientId (Guid, FK)
  ├── DoctorId (Guid, FK)
  ├── TimeSlotId (Guid, FK)
  ├── Status (enum: SCHEDULED, COMPLETED, CANCELLED)
  ├── Amount (Money)
  ├── MedicalDocumentId (Guid?, FK)
  ├── CreatedAt (datetime)
  └── UpdatedAt (datetime)

-- MedicalDocuments
MedicalDocuments
  ├── Id (Guid, PK)
  ├── PatientId (Guid, FK)
  ├── DoctorId (Guid, FK)
  ├── SpecialistId (Guid?, FK)
  ├── Diagnosis (string)
  ├── Prescription (string)
  ├── Total (Money)
  ├── PdfUrl (string)
  ├── CreatedAt (datetime)
  └── MedicalActs (ICollection)

-- MedicalActs
MedicalActs
  ├── Id (Guid, PK)
  ├── MedicalDocumentId (Guid, FK)
  ├── Type (enum: IRM, RADIOGRAPHIE, ECG, ANALYSE_SANG, etc.)
  ├── Price (Money)
  └── Notes (string?, optional)

-- Payments
Payments
  ├── Id (Guid, PK)
  ├── AppointmentId (Guid?, FK)
  ├── MedicalDocumentId (Guid?, FK)
  ├── Amount (Money)
  ├── Status (enum: PENDING, COMPLETED, FAILED)
  ├── StripeTransactionId (string)
  ├── CreatedAt (datetime)
  └── UpdatedAt (datetime)
```

---

### 9.2 Storage Externe

```
Documents Médicaux (PDF):
  ├── Cloud: Azure Blob Storage ou AWS S3
  ├── Nommage: {patientId}/{documentId}/{timestamp}.pdf
  ├── Chiffrement: AES-256
  ├── Rétention: 7 ans minimum

Images Patient:
  ├── Avatar: /images/patients/{patientId}/avatar.jpg
  ├── Documents scannés: /images/patients/{patientId}/documents/
  ├── Compression: WebP (50% moins volumineux)
  ├── Limite: 10MB/fichier

Logs:
  ├── Fichier: /var/log/medilink/
  ├── Rotation: 1 fichier/jour
  ├── Rétention: 30 jours (compliance)
```

---

## 10. INTÉGRATIONS EXTERNES {#integrations}

### 10.1 Stripe

**API Endpoints:**
```
POST https://api.stripe.com/v1/checkout/sessions
POST https://api.stripe.com/v1/webhooks

Webhook Events:
- payment_intent.succeeded
- payment_intent.payment_failed
- charge.refunded
```

**Flow:**
```
1. API → Créer checkout session
2. Frontend → Redirection Stripe
3. Client → Paiement sécurisé
4. Stripe → Webhook to API
5. API → Update payment status
6. SignalR → Notifier clients
```

---

### 10.2 Email (Phase 1: SendGrid ou SMTP)

**Emails:**
```
1. Confirmation Inscription
2. Confirmation Réservation
3. Rappel 24h avant RDV
4. Confirmation Consultation
5. Invoice/Facture
6. Réinitialisation Mot de passe
7. Notification Spécialiste (nouvelle demande)
```

**Template:**
```
Langue: Français + Arabe (future)
Branding: Logo hôpital
Footer: Infos de contact
```

---

### 10.3 SMS (Phase 2)

**Provider:** Twilio ou local (Maroc)

**Messages:**
```
- Confirmation RDV (court code)
- Rappel 1h avant
- Confirmation paiement
```

---

### 10.4 QuestPDF

**Documents Générés:**
```
1. Ordonnance Médicale
2. Fiche Consultation
3. Facture/Invoice
4. Dossier Médical Complet

Format:
- A4 Portrait
- PDF/A pour archivage légal
- Signature numérique
```

---

## 11. NORMES & CONFORMITÉ

```
✅ HIPAA (Health Insurance Portability)
✅ GDPR (si données EU)
✅ Standards Marocains (Santé)
✅ Archivage 7 ans (légal - invoices)
✅ Audit trail complète
✅ Conformité PCI DSS (Stripe)
✅ HTTPS/TLS obligatoire
```

---

## PHASES DE DÉPLOIEMENT

### **Phase 1 (MVP) - 3 mois**
- ✅ Module Patient
- ✅ Module Infirmier
- ✅ Module Médecin Généraliste
- ✅ Créneaux basiques
- ✅ Réservation simple
- ✅ Paiement (avance)

### **Phase 2 - 3 mois**
- 🔄 Module Spécialiste complet
- 🔄 Téléexpertise
- 🔄 SignalR notifications avancées
- 🔄 Rapports/Analytics

### **Phase 3 - 6 mois**
- 🔄 Vidéoconsultation
- 🔄 Mobile app native
- 🔄 BI/Dashboard analytique
- 🔄 Intégration labo (résultats)

---

**Document Approuvé par:**
- CTO: [Signature]
- PM: [Signature]
- Business: [Signature]

**Date:** 2024  
**Dernière Révision:** [À remplir]
