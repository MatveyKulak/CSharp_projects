using AutoMapper;
using Hospital.Business.Models.People;

namespace Hospital.WPF.Mapping
{
    /// <summary>
    /// Конфигурация AutoMapper для сущности Patient.
    /// </summary>
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // Настраивает маппинг для копирования данных из одного объекта Patient в другой.
            // Используется при обновлении данных пациента в PatientService.
            CreateMap<Patient, Patient>();
        }
    }
}