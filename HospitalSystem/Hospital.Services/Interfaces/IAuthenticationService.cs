using Hospital.Business.Models.People;

namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса аутентификации пользователей.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Аутентифицирует пользователя по логину и паролю.
        /// </summary>
        /// <returns>Объект User в случае успеха, иначе null.</returns>
        Task<User?> AuthenticateAsync(string username, string password);
    }
}