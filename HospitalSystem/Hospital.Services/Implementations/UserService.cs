using AutoMapper;
using Hospital.Business.Models.People;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует комплексную логику управления пользователями всех ролей.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Nurse> _nurseRepository;
        private readonly IRepository<Administrator> _adminRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(
            IRepository<Doctor> doctorRepository,
            IRepository<Nurse> nurseRepository,
            IRepository<Administrator> adminRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _nurseRepository = nurseRepository;
            _adminRepository = adminRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Теперь используем GetAllWithIncludeAsync для загрузки связанных данных
            var doctors = await _doctorRepository.GetAllWithIncludeAsync(doctor => doctor.Department);
            var nurses = await _nurseRepository.GetAllWithIncludeAsync(nurse => nurse.Department);
            var admins = await _adminRepository.GetAllAsync();

            var allUsers = new List<User>();
            allUsers.AddRange(doctors);
            allUsers.AddRange(nurses);
            allUsers.AddRange(admins);

            return allUsers.OrderBy(u => u.LastName);
        }

        public async Task<User> AddUserAsync(User user, string password, string role)
        {
            User newUser;
            switch (role)
            {
                case "Врач":
                    var doctor = new Doctor();
                    doctor.DepartmentId = user.TempDepartmentId; // Используем временное свойство
                    newUser = doctor;
                    break;
                case "Медсестра":
                    var nurse = new Nurse();
                    nurse.DepartmentId = user.TempDepartmentId;
                    newUser = nurse;
                    break;
                case "Администратор":
                    newUser = new Administrator();
                    break;
                default:
                    throw new ArgumentException("Неизвестная роль", nameof(role));
            }

            _mapper.Map(user, newUser);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = _passwordHasher.Hash(password);

            await AddToRepositoryAsync(newUser);
            return newUser;
        }

        public async Task UpdateUserAsync(User user, string? newPassword, string newRole)
        {
            var existingUser = await FindUserByIdAsync(user.Id);
            if (existingUser == null) return;

            if (existingUser.Role == newRole)
            {
                _mapper.Map(user, existingUser);
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    existingUser.PasswordHash = _passwordHasher.Hash(newPassword);
                }
                await UpdateInRepositoryAsync(existingUser);
            }
            else
            {
                // Смена роли - это сложная операция, требующая перемещения между таблицами.
                // Подход "удалить-создать" является самым простым и надежным в данном случае.
                await DeleteUserAsync(user.Id);

                user.PasswordHash = !string.IsNullOrWhiteSpace(newPassword)
                    ? _passwordHasher.Hash(newPassword)
                    : existingUser.PasswordHash;

                await AddUserAsync(user, "dummyPassword", newRole);
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var userToDelete = await FindUserByIdAsync(userId);
            if (userToDelete == null) return;

            if (userToDelete is Administrator admin)
            {
                var adminCount = (await _adminRepository.GetAllAsync()).Count();
                if (adminCount <= 1)
                {
                    // Важное бизнес-правило: нельзя оставить систему без администратора.
                    throw new InvalidOperationException("Нельзя удалить единственного администратора.");
                }
                _adminRepository.Remove(admin);
                await _adminRepository.SaveChangesAsync();
            }
            else if (userToDelete is Doctor doctor)
            {
                _doctorRepository.Remove(doctor);
                await _doctorRepository.SaveChangesAsync();
            }
            else if (userToDelete is Nurse nurse)
            {
                _nurseRepository.Remove(nurse);
                await _nurseRepository.SaveChangesAsync();
            }
        }

        private async Task<User?> FindUserByIdAsync(Guid userId)
        {
            User? user = await _doctorRepository.GetByIdAsync(userId);
            if (user != null) return user;
            user = await _nurseRepository.GetByIdAsync(userId);
            if (user != null) return user;
            user = await _adminRepository.GetByIdAsync(userId);
            return user;
        }

        private async Task AddToRepositoryAsync(User user)
        {
            if (user is Doctor doctor) { await _doctorRepository.AddAsync(doctor); await _doctorRepository.SaveChangesAsync(); }
            else if (user is Nurse nurse) { await _nurseRepository.AddAsync(nurse); await _nurseRepository.SaveChangesAsync(); }
            else if (user is Administrator admin) { await _adminRepository.AddAsync(admin); await _adminRepository.SaveChangesAsync(); }
        }

        private async Task UpdateInRepositoryAsync(User user)
        {
            if (user is Doctor doctor) { _doctorRepository.Update(doctor); await _doctorRepository.SaveChangesAsync(); }
            else if (user is Nurse nurse) { _nurseRepository.Update(nurse); await _nurseRepository.SaveChangesAsync(); }
            else if (user is Administrator admin) { _adminRepository.Update(admin); await _adminRepository.SaveChangesAsync(); }
        }
    }
}