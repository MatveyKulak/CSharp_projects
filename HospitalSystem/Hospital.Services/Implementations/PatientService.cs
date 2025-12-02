using AutoMapper;
using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует сервис для управления данными пациентов.
    /// </summary>
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IMapper _mapper;

        public PatientService(IRepository<Patient> patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            // Загружаем все необходимые связанные данные одним запросом для избежания проблемы N+1.
            return await _patientRepository.GetAllWithIncludeAsync(
                patient => patient.MedicalRecord.Appointments,
                patient => patient.AssignedDoctor,
                patient => patient.Department); // Добавил Department для полноты
        }

        public async Task AddPatientAsync(Patient patient)
        {
            // Бизнес-логика: при создании пациента автоматически создается и связывается мед. карта.
            patient.Id = Guid.NewGuid();
            patient.MedicalRecord = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                HospitalizationDate = DateTime.Now
            };
            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            // Правильный паттерн обновления: сначала загружаем, потом изменяем.
            var existingPatient = await _patientRepository.GetByIdAsync(patient.Id);
            if (existingPatient != null)
            {
                _mapper.Map(patient, existingPatient);
                _patientRepository.Update(existingPatient);
                await _patientRepository.SaveChangesAsync();
            }
        }

        public async Task DeletePatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient != null)
            {
                _patientRepository.Remove(patient);
                await _patientRepository.SaveChangesAsync();
            }
        }
    }
}