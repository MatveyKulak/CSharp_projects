using Hospital.Business.Models;

namespace Hospital.Services.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса управления отделениями.
    /// </summary>
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> AddDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(Guid departmentId);
    }
}