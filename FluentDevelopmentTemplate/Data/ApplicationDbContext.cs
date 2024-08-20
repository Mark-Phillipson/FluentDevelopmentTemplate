using FluentDevelopmentTemplate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FluentDevelopmentTemplate.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : IdentityDbContext<ApplicationUser>(options)
    {
        private readonly IConfiguration _configuration = configuration;

        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (_configuration != null)
                {
                    optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                }
            }
        }
    }
}
