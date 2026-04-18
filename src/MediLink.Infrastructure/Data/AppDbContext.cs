namespace MediLink.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;

/// <summary>
/// Main Database Context for MediLink application
/// Supports SQL Server and PostgreSQL
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    #region DbSets - Users
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Specialist> Specialists { get; set; } = null!;
    public DbSet<Nurse> Nurses { get; set; } = null!;
    public DbSet<Admin> Admins { get; set; } = null!;
    #endregion

    #region DbSets - Medical
    public DbSet<TimeSlot> TimeSlots { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<MedicalDocument> MedicalDocuments { get; set; } = null!;
    public DbSet<MedicalAct> MedicalActs { get; set; } = null!;
    public DbSet<BlockedDay> BlockedDays { get; set; } = null!;
    #endregion

    #region DbSets - Payments
    public DbSet<Payment> Payments { get; set; } = null!;
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TPH (Table-Per-Hierarchy) inheritance for User
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Specialist>("Specialist")
            .HasValue<Nurse>("Nurse")
            .HasValue<Admin>("Admin");

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
        });

        // Configure Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.CIN).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.BloodType).HasMaxLength(5);
            entity.Property(e => e.Allergies).HasMaxLength(500);
            entity.OwnsOne(e => e.Address);
        });

        // Configure Doctor
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CabinetName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CabinetPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CabinetAddress).IsRequired().HasMaxLength(500);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.ConsultationPrice).HasPrecision(10, 2);
            entity.HasIndex(e => e.IsVerified);
        });

        // Configure Specialist
        modelBuilder.Entity<Specialist>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HospitalName).HasMaxLength(200);
            entity.Property(e => e.HospitalPhone).HasMaxLength(20);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.ConsultationPrice).HasPrecision(10, 2);
        });

        // Configure Nurse
        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.License).HasMaxLength(50);
            entity.HasOne(e => e.Doctor).WithMany().HasForeignKey(e => e.DoctorId);
        });

        // Configure Admin
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Department).HasMaxLength(100);
        });

        // Configure TimeSlot
        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasOne(e => e.Doctor).WithMany(d => d.TimeSlots).HasForeignKey(e => e.DoctorId);
            entity.HasOne(e => e.Appointment).WithOne(a => a.TimeSlot).HasForeignKey<Appointment>(a => a.TimeSlotId);
            entity.HasIndex(e => new { e.DoctorId, e.Date, e.Status }).IsUnique(false);
            entity.HasIndex(e => e.Status);
        });

        // Configure Appointment
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symptoms).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.HasOne(e => e.Patient).WithMany(p => p.Appointments).HasForeignKey(e => e.PatientId);
            entity.HasOne(e => e.Doctor).WithMany(d => d.Appointments).HasForeignKey(e => e.DoctorId);
            entity.HasIndex(e => new { e.PatientId, e.Status });
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure MedicalDocument
        modelBuilder.Entity<MedicalDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Diagnosis).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Prescription).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.PdfUrl).HasMaxLength(500);
            entity.Property(e => e.Total).HasPrecision(10, 2);
            entity.HasOne(e => e.Patient).WithMany(p => p.MedicalDocuments).HasForeignKey(e => e.PatientId);
            entity.HasOne(e => e.Doctor).WithMany(d => d.MedicalDocuments).HasForeignKey(e => e.DoctorId);
            entity.HasOne(e => e.Specialist).WithMany(s => s.MedicalDocuments).HasForeignKey(e => e.SpecialistId);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.IsArchived);
        });

        // Configure MedicalAct
        modelBuilder.Entity<MedicalAct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.Result).HasMaxLength(1000);
            entity.HasOne(e => e.MedicalDocument).WithMany(d => d.MedicalActs).HasForeignKey(e => e.MedicalDocumentId);
        });

        // Configure Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.StripeTransactionId).HasMaxLength(100);
            entity.Property(e => e.StripeSessionId).HasMaxLength(200);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.HasOne(e => e.Appointment).WithOne(a => a.Payment).HasForeignKey<Payment>(e => e.AppointmentId);
            entity.HasOne(e => e.MedicalDocument).WithOne().HasForeignKey<Payment>(e => e.MedicalDocumentId);
            entity.HasOne(e => e.Patient).WithMany(p => p.Payments).HasForeignKey(e => e.PatientId);
            entity.HasIndex(e => e.StripeTransactionId).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PatientId);
        });

        // Configure BlockedDay
        modelBuilder.Entity<BlockedDay>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasOne(e => e.Doctor).WithMany(d => d.BlockedDays).HasForeignKey(e => e.DoctorId);
            entity.HasIndex(e => new { e.DoctorId, e.Date });
        });

        // Set default values
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.FindPrimaryKey()?.Properties;
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (property.ClrType == typeof(Guid))
                    {
                        property.SetDefaultValueSql("NEWID()");
                    }
                }
            }
        }
    }
}
