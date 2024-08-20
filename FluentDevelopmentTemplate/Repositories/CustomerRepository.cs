
using System;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FluentDevelopmentTemplate.Data;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Models;
using FluentDevelopmentTemplate.Repositories;

namespace FluentDevelopmentTemplate.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CustomerRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            this._mapper = mapper;
        }
        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            using var context = _contextFactory.CreateDbContext();
            List<Customer> Customers;
            if (!string.IsNullOrWhiteSpace(serverSearchTerm))
            {
                Customers = await context.Customers
                                        .Where(v =>
                    (v.Name != null && v.Name.ToLower().Contains(serverSearchTerm))
                    )

                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                Customers = await context.Customers
                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            IEnumerable<CustomerDTO> CustomersDTO = _mapper.Map<List<Customer>, IEnumerable<CustomerDTO>>(Customers);
            return CustomersDTO;
        }
        public async Task<IEnumerable<CustomerDTO>> SearchCustomersAsync(string serverSearchTerm)
        {
            using var context = _contextFactory.CreateDbContext();
            var Customers = await context.Customers
                //.Where(v => v.Property!= null  && v.Property.ToLower().Contains(serverSearchTerm.ToLower())
                //||v.Property!= null  && v.Property.ToLower().Contains(serverSearchTerm.ToLower())
                //)
                //.OrderBy(v => v.?)
                .Take(1000)
                .ToListAsync();
            IEnumerable<CustomerDTO> CustomersDTO = _mapper.Map<List<Customer>, IEnumerable<CustomerDTO>>(Customers);
            return CustomersDTO;
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int Id)
        {
            using var context = _contextFactory.CreateDbContext();
            var result = await context.Customers.AsNoTracking()
              .FirstOrDefaultAsync(c => c.Id == Id);
            if (result == null) return null;
            CustomerDTO customerDTO = _mapper.Map<Customer, CustomerDTO>(result);
            return customerDTO;
        }

        public async Task<CustomerDTO?> AddCustomerAsync(CustomerDTO customerDTO)
        {
            using var context = _contextFactory.CreateDbContext();
            Customer customer = _mapper.Map<CustomerDTO, Customer>(customerDTO);
            var addedEntity = context.Customers.Add(customer);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
            CustomerDTO resultDTO = _mapper.Map<Customer, CustomerDTO>(customer);
            return resultDTO;
        }

        public async Task<CustomerDTO?> UpdateCustomerAsync(CustomerDTO customerDTO)
        {
            Customer customer = _mapper.Map<CustomerDTO, Customer>(customerDTO);
            using (var context = _contextFactory.CreateDbContext())
            {
                var foundCustomer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(e => e.Id == customer.Id);

                if (foundCustomer != null)
                {
                    var mappedCustomer = _mapper.Map<Customer>(customer);
                    context.Customers.Update(mappedCustomer);
                    await context.SaveChangesAsync();
                    CustomerDTO resultDTO = _mapper.Map<Customer, CustomerDTO>(mappedCustomer);
                    return resultDTO;
                }
            }
            return null;
        }
        public async Task DeleteCustomerAsync(int Id)
        {
            using var context = _contextFactory.CreateDbContext();
            var foundCustomer = context.Customers.FirstOrDefault(e => e.Id == Id);
            if (foundCustomer == null)
            {
                return;
            }
            context.Customers.Remove(foundCustomer);
            await context.SaveChangesAsync();
        }
        public async Task<int> GetTotalCountAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Customers.CountAsync();
        }

        public Task<IQueryable<CustomerDTO>> GetAllCustomersIQueryableAsync(int pageNumber, int pageSize, string? serverSearchTerm)
        {
            using var context = _contextFactory.CreateDbContext();
            IQueryable<CustomerDTO> CustomersDTO;
            if (!string.IsNullOrWhiteSpace(serverSearchTerm))
            {
                CustomersDTO = context.Customers
                    .Where(v =>
                    (v.Name != null && v.Name.ToLower().Contains(serverSearchTerm))
                    )
                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CustomerDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                    });
            }
            else
            {
                CustomersDTO = context.Customers
                    //.OrderBy(v => v.?)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CustomerDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                    });
            }
            return Task.FromResult(CustomersDTO);

        }
    }
}