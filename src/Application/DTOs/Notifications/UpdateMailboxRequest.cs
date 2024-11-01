using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notifications;

public class UpdateMailboxRequest
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Status { get; set; } = null!;
    
    public double MailWeight { get; set; }
}