using AutoMapper;
using Hospital.Business.Models.People;

namespace Hospital.WPF.Mapping
{
    /// <summary>
    /// Конфигурация AutoMapper для иерархии пользователей (User).
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Настраивает базовый маппинг для User и автоматически распространяет его
            // на всех наследников (Doctor, Nurse, Administrator).
            CreateMap<User, User>()
                .IncludeAllDerived();
        }
    }
}