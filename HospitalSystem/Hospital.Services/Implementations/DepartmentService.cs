using Hospital.Business.Models;
using Hospital.Data.Repositories;
using Hospital.Services.Interfaces;

namespace Hospital.Services.Implementations
{
    /// <summary>
    /// Реализует сервис для выполнения CRUD-операций над отделениями.
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            department.Id = Guid.NewGuid(); // Бизнес-логика: сервис отвечает за генерацию ID
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return department;
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department != null)
            {
                _departmentRepository.Remove(department);
                await _departmentRepository.SaveChangesAsync();
            }
        }
    }
}