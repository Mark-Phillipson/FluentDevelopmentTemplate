using System;
using System.ComponentModel.DataAnnotations;

namespace FluentDevelopmentTemplate.Models;

public class Customer
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Name { get; set; } = default!;
}
