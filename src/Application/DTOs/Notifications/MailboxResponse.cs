namespace Application.DTOs.Notifications;

public class MailboxResponse
{
    public int Id { get; set; }
    public string Password { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public double? MailWeight { get; set; }
}