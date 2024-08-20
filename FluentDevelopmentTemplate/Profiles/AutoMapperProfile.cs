using AutoMapper;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Models;

namespace BlazorApp.Template
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<EmployeeDTO, Employee>();
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
        }
    }
}
