using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notifications;

public class CreateMailboxRequest
{
    [Required]
    [StringLength(255)]
    public string Password { get; set; } = null!;
    
    [Required]
    [StringLength(255)]
    public string Status { get; set; } = null!;
    
    public double MailWeight { get; set; }
}