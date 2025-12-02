namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса хэширования и верификации паролей.
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hashedPassword);
    }
}