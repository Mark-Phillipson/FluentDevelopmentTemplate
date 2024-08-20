using AutoMapper;
using Ardalis.GuardClauses;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FluentDevelopmentTemplate.Services
{
    public class CustomerDataService : ICustomerDataService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerDataService(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }
        public async Task<List<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            var Customers = await _customerRepository.GetAllCustomersAsync(pageNumber, pageSize, serverSearchTerm);
            return Customers.ToList();
        }
        public async Task<List<CustomerDTO>> SearchCustomersAsync(string serverSearchTerm)
        {
            var Customers = await _customerRepository.SearchCustomersAsync(serverSearchTerm);
            return Customers.ToList();
        }

        public async Task<CustomerDTO?> GetCustomerById(int Id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(Id);
            return customer;
        }
        public async Task<CustomerDTO?> AddCustomer(CustomerDTO customerDTO)
        {
            Guard.Against.Null(customerDTO);
            var result = await _customerRepository.AddCustomerAsync(customerDTO);
            if (result == null)
            {
                throw new Exception($"Add of customer failed ID: {customerDTO.Id}");
            }
            return result;
        }
        public async Task<CustomerDTO> UpdateCustomer(CustomerDTO customerDTO, string? username)
        {
            Guard.Against.Null(customerDTO);
            Guard.Against.Null(username);
            var result = await _customerRepository.UpdateCustomerAsync(customerDTO);
            if (result == null)
            {
                throw new Exception($"Update of customer failed ID: {customerDTO.Id}");
            }
            return result;
        }

        public async Task DeleteCustomer(int Id)
        {
            await _customerRepository.DeleteCustomerAsync(Id);
        }
        public async Task<int> GetTotalCount()
        {
            int result = await _customerRepository.GetTotalCountAsync();
            return result;
        }

        public async Task<IQueryable<CustomerDTO>> GetAllCustomersIQueryableAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            IQueryable<CustomerDTO> result = await _customerRepository.GetAllCustomersIQueryableAsync(pageNumber, pageSize, serverSearchTerm);
            return result;
        }
    }
}