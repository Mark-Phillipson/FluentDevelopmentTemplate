
using System.ComponentModel.DataAnnotations;

namespace FluentDevelopmentTemplate.DTOs
{
    public partial class EmployeeDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
        [StringLength(50)]
        public string? Department { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? PhotoPath { get; set; }
    }
}