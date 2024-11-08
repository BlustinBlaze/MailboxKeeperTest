using API;
using Application.DTOs.Notifications;

namespace Application.Interfaces;

public interface IMailboxRepository
{
    List<HistoryResponse> GetMailboxes(string userId);
    MailboxResponse GetMailboxById(int id);
    void CreateMailbox(CreateMailboxRequest createMailboxRequest);
    MailboxResponse UpdateMailbox(UpdateMailboxRequest mailbox);
}