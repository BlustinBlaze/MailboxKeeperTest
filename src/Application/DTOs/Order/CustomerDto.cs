using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Order;

public class CustomerDto
{
    [Required]
    [StringLength(255)]
    public string Firstname { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Lastname { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }
}