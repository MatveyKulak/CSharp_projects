using Hospital.Business.Models.People;

namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса управления пользователями.
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AddUserAsync(User user, string password, string role);
        Task UpdateUserAsync(User user, string? newPassword, string newRole);
        Task DeleteUserAsync(Guid userId);
    }
}