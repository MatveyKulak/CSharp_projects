using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует сервис аутентификации, проверяя учетные данные пользователя.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Nurse> _nurseRepository;
        private readonly IRepository<Administrator> _adminRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(
            IRepository<Doctor> doctorRepository,
            IRepository<Nurse> nurseRepository,
            IRepository<Administrator> adminRepository,
            IPasswordHasher passwordHasher)
        {
            _doctorRepository = doctorRepository;
            _nurseRepository = nurseRepository;
            _adminRepository = adminRepository;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Пытается аутентифицировать пользователя по имени и паролю.
        /// </summary>
        /// <returns>Объект User в случае успеха, иначе null.</returns>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            // Улучшено именование переменных в лямбда-выражениях
            var doctor = (await _doctorRepository.FindAsync(doc => doc.Username == username)).FirstOrDefault();
            if (doctor != null)
            {
                return _passwordHasher.Verify(password, doctor.PasswordHash) ? doctor : null;
            }

            var nurse = (await _nurseRepository.FindAsync(nurse => nurse.Username == username)).FirstOrDefault();
            if (nurse != null)
            {
                return _passwordHasher.Verify(password, nurse.PasswordHash) ? nurse : null;
            }

            var admin = (await _adminRepository.FindAsync(admin => admin.Username == username)).FirstOrDefault();
            if (admin != null)
            {
                return _passwordHasher.Verify(password, admin.PasswordHash) ? admin : null;
            }

            return null;
        }
    }
}