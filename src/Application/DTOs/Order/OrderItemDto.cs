using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Order;

public class OrderItemDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
    public decimal Price { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number greater than 0")]
    public int Quantity { get; set; }
}