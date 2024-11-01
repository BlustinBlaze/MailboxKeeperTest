using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Order;

public class OrderDto
{
    [Required]
    [StringLength(255)]
    public string Country { get; set; }
    
    [Required]
    [StringLength(512)]
    public string Street { get; set; }
    
    [Required]
    [StringLength(255)]
    public string City { get; set; }
    
    //[RegularExpression(@"^[A-Z]\d[A-Z]\d[A-Z]\d$", ErrorMessage = "Invalid zip code")]
    [Required]
    [StringLength(6)]
    public string ZipCode { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Total must be a positive number")]
    public decimal Total { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<OrderItemDto> Items { get; set; }
}