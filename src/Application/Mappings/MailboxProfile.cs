using API;
using Application.DTOs.Notifications;
using AutoMapper;

namespace Application.Mappings;

public class MailboxProfile: Profile
{
    public MailboxProfile()
    {
        //CreateMap<MailboxResponse, Mailbox>();
        CreateMap<Mailbox, MailboxResponse>();
        CreateMap<CreateMailboxRequest, Mailbox>();
        CreateMap<History, HistoryResponse>();
    }
}