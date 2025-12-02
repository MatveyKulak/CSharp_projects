using Hospital.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Data.Factories
{
    /// <summary>
    /// Фабрика для создания экземпляра ApplicationDbContext во время разработки.
    /// Используется инструментами EF Core (например, при создании миграций) для получения
    /// сконфигурированного контекста базы данных без запуска основного приложения.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Строка подключения жестко задана, так как этот код выполняется только
            // на машине разработчика в процессе создания миграций.
            optionsBuilder.UseSqlite("Data Source=hospital.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}