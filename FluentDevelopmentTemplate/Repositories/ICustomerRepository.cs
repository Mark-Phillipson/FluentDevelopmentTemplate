
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentDevelopmentTemplate.DTOs;

namespace FluentDevelopmentTemplate.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerDTO?> AddCustomerAsync(CustomerDTO customerDTO);
        Task DeleteCustomerAsync(int Id);
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize, string? serverSearchTerm);
        Task<IEnumerable<CustomerDTO>> SearchCustomersAsync(string serverSearchTerm);
        Task<CustomerDTO?> GetCustomerByIdAsync(int Id);
        Task<CustomerDTO?> UpdateCustomerAsync(CustomerDTO customerDTO);
        Task<int> GetTotalCountAsync();
        Task<IQueryable<CustomerDTO>> GetAllCustomersIQueryableAsync(int pageNumber, int pageSize, string? serverSearchTerm);
    }
}