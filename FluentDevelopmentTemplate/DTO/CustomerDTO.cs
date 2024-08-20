
using System.ComponentModel.DataAnnotations;

namespace FluentDevelopmentTemplate.DTOs
{
    public partial class CustomerDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
    }
}