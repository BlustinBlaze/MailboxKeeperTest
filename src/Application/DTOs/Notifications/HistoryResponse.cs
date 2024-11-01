namespace Application.DTOs.Notifications;

public class HistoryResponse
{
    public int Id { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Time { get; set; }
    public double MailWeight { get; set; }
    public int IdMailbox { get; set; }
}