using Application.DTOs.Notifications;

namespace API;

public interface INotificationClient
{
    Task ReceiveHistory(List<HistoryResponse> mailbox);
    Task ReceiveMailbox(MailboxResponse mailbox);
    Task ReceiveMailboxUpdate(MailboxResponse mailbox);
}