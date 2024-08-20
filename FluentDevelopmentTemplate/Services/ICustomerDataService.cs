

using FluentDevelopmentTemplate.DTOs;

namespace FluentDevelopmentTemplate.Services
{
    public interface ICustomerDataService
    {
        Task<List<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize, string? serverSearchTerm);
        Task<List<CustomerDTO>> SearchCustomersAsync(string serverSearchTerm);
        Task<CustomerDTO?> AddCustomer(CustomerDTO customerDTO);
        Task<CustomerDTO?> GetCustomerById(int Id);
        Task<CustomerDTO> UpdateCustomer(CustomerDTO customerDTO, string? username);
        Task DeleteCustomer(int Id);
        Task<int> GetTotalCount();
        Task<IQueryable<CustomerDTO>> GetAllCustomersIQueryableAsync(int pageNumber, int pageSize, string? serverSearchTerm);
    }
}