using Ardalis.GuardClauses;
using AutoMapper;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FluentDevelopmentTemplate.Services
{
    public class EmployeeDataService : IEmployeeDataService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeDataService(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }
        public async Task<List<EmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            var Employees = await _employeeRepository.GetAllEmployeesAsync(pageNumber, pageSize, serverSearchTerm);
            return Employees.ToList();
        }
        public async Task<List<EmployeeDTO>> SearchEmployeesAsync(string serverSearchTerm)
        {
            var Employees = await _employeeRepository.SearchEmployeesAsync(serverSearchTerm);
            return Employees.ToList();
        }

        public async Task<EmployeeDTO?> GetEmployeeById(int Id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(Id);
            return employee;
        }
        public async Task<EmployeeDTO?> AddEmployee(EmployeeDTO employeeDTO)
        {
            Guard.Against.Null(employeeDTO);
            var result = await _employeeRepository.AddEmployeeAsync(employeeDTO);
            if (result == null)
            {
                throw new Exception($"Add of employee failed ID: {employeeDTO.Id}");
            }
            return result;
        }
        public async Task<EmployeeDTO> UpdateEmployee(EmployeeDTO employeeDTO, string? username)
        {
            Guard.Against.Null(employeeDTO);
            Guard.Against.Null(username);
            var result = await _employeeRepository.UpdateEmployeeAsync(employeeDTO);
            if (result == null)
            {
                throw new Exception($"Update of employee failed ID: {employeeDTO.Id}");
            }
            return result;
        }

        public async Task DeleteEmployee(int Id)
        {
            await _employeeRepository.DeleteEmployeeAsync(Id);
        }
        public async Task<int> GetTotalCount()
        {
            int result = await _employeeRepository.GetTotalCountAsync();
            return result;
        }
    }
}