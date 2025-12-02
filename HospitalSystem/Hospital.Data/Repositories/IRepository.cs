using System.Linq.Expressions;

namespace Hospital.Data.Repositories
{
    /// <summary>
    /// Определяет контракт для обобщенного репозитория, предоставляющего
    /// стандартные операции для работы с сущностями (CRUD), включая загрузку связанных данных.
    /// </summary>
    /// <typeparam name="T">Тип сущности, с которым работает репозиторий.</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetAll();

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        Task<int> SaveChangesAsync();

        Task<T?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includeProperties);
    }
}