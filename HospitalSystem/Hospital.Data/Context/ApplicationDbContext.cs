using Hospital.Business.Models;
using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Data.Context
{
    /// <summary>
    /// Основной класс контекста базы данных для Entity Framework Core.
    /// Представляет сессию с базой данных и позволяет запрашивать и сохранять данные.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        // Наборы сущностей, которые будут преобразованы в таблицы в базе данных.
        public DbSet<Department> Departments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DiagnosticAppointment> DiagnosticAppointments { get; set; }
        public DbSet<ProphylacticAppointment> ProphylacticAppointments { get; set; }
        public DbSet<MedicationAppointment> MedicationAppointments { get; set; }
        public DbSet<InjectionAppointment> InjectionAppointments { get; set; }
        public DbSet<PillAppointment> PillAppointments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Настраивает модель данных и её связи с помощью Fluent API.
        /// Вызывается один раз при первом создании экземпляра контекста.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- ДОБАВЛЕНЫ НАСТРОЙКИ СВЯЗЕЙ ---

            // Настройка связи "один-ко-многим": Отделение -> Врачи
            modelBuilder.Entity<Department>()
                .HasMany(department => department.Doctors)      // У Отделения много Врачей
                .WithOne(doctor => doctor.Department)           // У Врача одно Отделение
                .HasForeignKey(doctor => doctor.DepartmentId)   // Внешний ключ в таблице Врачей
                .OnDelete(DeleteBehavior.SetNull); // При удалении Отделения, у Врачей DepartmentId станет null (они "открепятся")

            // Настройка связи "один-ко-многим": Отделение -> Пациенты
            modelBuilder.Entity<Department>()
                .HasMany(department => department.Patients)
                .WithOne(patient => patient.Department)
                .HasForeignKey(patient => patient.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull); // Аналогично врачам

            // Настройка связи "один-ко-многим": Врач -> Пациенты
            modelBuilder.Entity<Doctor>()
                .HasMany(doctor => doctor.Patients)
                .WithOne(patient => patient.AssignedDoctor)
                .HasForeignKey(patient => patient.DoctorId)
                .OnDelete(DeleteBehavior.SetNull); // При удалении (увольнении) врача, его пациенты не удаляются, а просто "открепляются"

            // Настройка связи "один-к-одному": Пациент -> Мед. карта
            modelBuilder.Entity<Patient>()
                .HasOne(patient => patient.MedicalRecord)
                .WithOne(medicalRecord => medicalRecord.Patient)
                .HasForeignKey<MedicalRecord>(medicalRecord => medicalRecord.PatientId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении Пациента его Мед. карта тоже удалится (каскадное удаление)

            // Настройка связи "один-ко-многим": Мед. карта -> Назначения
            modelBuilder.Entity<MedicalRecord>()
                .HasMany(medicalRecord => medicalRecord.Appointments)
                .WithOne(appointment => appointment.MedicalRecord)
                .HasForeignKey(appointment => appointment.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении Мед. карты все связанные с ней Назначения тоже удалятся
        }
    }
}