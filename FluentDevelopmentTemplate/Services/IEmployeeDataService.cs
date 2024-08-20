
using FluentDevelopmentTemplate.DTOs;

namespace FluentDevelopmentTemplate.Services
{
    public interface IEmployeeDataService
    {
        Task<List<EmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize, string? serverSearchTerm);
        Task<List<EmployeeDTO>> SearchEmployeesAsync(string serverSearchTerm);
        Task<EmployeeDTO?> AddEmployee(EmployeeDTO employeeDTO);
        Task<EmployeeDTO?> GetEmployeeById(int Id);
        Task<EmployeeDTO> UpdateEmployee(EmployeeDTO employeeDTO, string? username);
        Task DeleteEmployee(int Id);
        Task<int> GetTotalCount();
    }
}