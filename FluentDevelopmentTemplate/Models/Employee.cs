using System.ComponentModel.DataAnnotations;

namespace FluentDevelopmentTemplate.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "Name is too long 50 characters maximum.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(50, ErrorMessage = "Department is too long 50 characters maximum.")]
        public string? Department { get; set; }
        [StringLength(50, ErrorMessage = "Email is too long 50 characters maximum.")]
        public string? Email { get; set; }
        [StringLength(255, ErrorMessage = "PhotoPath is too long 255 characters maximum.")]
        public string? PhotoPath { get; set; }

    }
}
