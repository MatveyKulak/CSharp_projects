using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует IPasswordHasher, используя надежную библиотеку BCrypt.Net для хэширования.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Создает хэш пароля.
        /// </summary>
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Проверяет, соответствует ли предоставленный пароль хэшу.
        /// </summary>
        public bool Verify(string password, string hashedPassword)
        {
            try
            {
                // BCrypt.Verify может выбросить исключение, если хэш имеет неверный формат.
                // Оборачиваем в try-catch, чтобы в любом случае вернуть false при ошибке.
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}