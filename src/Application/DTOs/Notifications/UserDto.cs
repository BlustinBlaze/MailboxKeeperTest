using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notifications;

public class UserDto
{
    [Required]
    [StringLength(255)]
    public string Email { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    public bool? Notification { get; set; }
    
    public int IdMailbox { get; set; }
}