
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FluentDevelopmentTemplate.Data;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Models;

namespace FluentDevelopmentTemplate.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public EmployeeRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            this._mapper = mapper;
        }
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            using var context = _contextFactory.CreateDbContext();
            List<Employee> Employees;
            if (!string.IsNullOrWhiteSpace(serverSearchTerm))
            {
                Employees = await context.Employees
                                        .Where(v =>
                    (v.Name != null && v.Name.ToLower().Contains(serverSearchTerm))
                     || (v.Department != null && v.Department.ToLower().Contains(serverSearchTerm))
                     || (v.Email != null && v.Email.ToLower().Contains(serverSearchTerm))
                     || (v.PhotoPath != null && v.PhotoPath.ToLower().Contains(serverSearchTerm))
                    )

                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                Employees = await context.Employees
                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            IEnumerable<EmployeeDTO> EmployeesDTO = _mapper.Map<List<Employee>, IEnumerable<EmployeeDTO>>(Employees);
            return EmployeesDTO;
        }
        public async Task<IEnumerable<EmployeeDTO>> SearchEmployeesAsync(string serverSearchTerm)
        {
            using var context = _contextFactory.CreateDbContext();
            var Employees = await context.Employees
                //.Where(v => v.Property!= null  && v.Property.ToLower().Contains(serverSearchTerm.ToLower())
                //||v.Property!= null  && v.Property.ToLower().Contains(serverSearchTerm.ToLower())
                //)
                //.OrderBy(v => v.?)
                .Take(1000)
                .ToListAsync();
            IEnumerable<EmployeeDTO> EmployeesDTO = _mapper.Map<List<Employee>, IEnumerable<EmployeeDTO>>(Employees);
            return EmployeesDTO;
        }

        public async Task<EmployeeDTO?> GetEmployeeByIdAsync(int Id)
        {
            using var context = _contextFactory.CreateDbContext();
            var result = await context.Employees.AsNoTracking()
              .FirstOrDefaultAsync(c => c.Id == Id);
            if (result == null) return null;
            EmployeeDTO employeeDTO = _mapper.Map<Employee, EmployeeDTO>(result);
            return employeeDTO;
        }

        public async Task<EmployeeDTO?> AddEmployeeAsync(EmployeeDTO employeeDTO)
        {
            using var context = _contextFactory.CreateDbContext();
            Employee employee = _mapper.Map<EmployeeDTO, Employee>(employeeDTO);
            var addedEntity = context.Employees.Add(employee);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
            EmployeeDTO resultDTO = _mapper.Map<Employee, EmployeeDTO>(employee);
            return resultDTO;
        }

        public async Task<EmployeeDTO?> UpdateEmployeeAsync(EmployeeDTO employeeDTO)
        {
            Employee employee = _mapper.Map<EmployeeDTO, Employee>(employeeDTO);
            using (var context = _contextFactory.CreateDbContext())
            {
                var foundEmployee = await context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employee.Id);

                if (foundEmployee != null)
                {
                    var mappedEmployee = _mapper.Map<Employee>(employee);
                    context.Employees.Update(mappedEmployee);
                    await context.SaveChangesAsync();
                    EmployeeDTO resultDTO = _mapper.Map<Employee, EmployeeDTO>(mappedEmployee);
                    return resultDTO;
                }
            }
            return null;
        }
        public async Task DeleteEmployeeAsync(int Id)
        {
            using var context = _contextFactory.CreateDbContext();
            var foundEmployee = context.Employees.FirstOrDefault(e => e.Id == Id);
            if (foundEmployee == null)
            {
                return;
            }
            context.Employees.Remove(foundEmployee);
            await context.SaveChangesAsync();
        }
        public async Task<int> GetTotalCountAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Employees.CountAsync();
        }

    }
}