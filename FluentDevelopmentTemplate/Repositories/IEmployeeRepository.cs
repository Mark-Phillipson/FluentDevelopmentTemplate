

using FluentDevelopmentTemplate.DTOs;

namespace FluentDevelopmentTemplate.Repositories
{
    public interface IEmployeeRepository
    {
        Task<EmployeeDTO?> AddEmployeeAsync(EmployeeDTO employeeDTO);
        Task DeleteEmployeeAsync(int Id);
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize, string? serverSearchTerm);
        Task<IEnumerable<EmployeeDTO>> SearchEmployeesAsync(string serverSearchTerm);
        Task<EmployeeDTO?> GetEmployeeByIdAsync(int Id);
        Task<EmployeeDTO?> UpdateEmployeeAsync(EmployeeDTO employeeDTO);
        Task<int> GetTotalCountAsync();
    }
}