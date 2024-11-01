using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notifications;

public class CreateHistoryRequest
{
    [Required]
    [StringLength(255)]
    public string Status { get; set; } = null!;
    
    [Required]
    public DateTime Time { get; set; }
    
    [Required]
    public double MailWeight { get; set; }
    
    [Required]
    public int IdMailbox { get; set; }
}