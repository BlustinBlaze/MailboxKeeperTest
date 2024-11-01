using API;
using Application.DTOs.Notifications;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class MailboxRepository(ApiContext db, IMapper mapper, ILogger<MailboxRepository> logger) : IMailboxRepository
{
    //private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1);
    public List<HistoryResponse> GetMailboxes()
    {
        return db.Histories.Select(history => mapper.Map<HistoryResponse>(history)).ToList();
    }
    
    public MailboxResponse GetMailboxById(int id)
    {
        var mailbox = db.Mailboxes.Find(id);
        if (mailbox == null)
        {
            throw new NotFoundException("Mailbox not found");
        }
        return mapper.Map<MailboxResponse>(mailbox);
    }
    
    public void CreateMailbox(CreateMailboxRequest createMailboxRequest)
    {
        logger.LogInformation("Adding mailbox to database...");
        var mailbox = mapper.Map<Mailbox>(createMailboxRequest);
        db.Mailboxes.Add(mailbox);
        db.SaveChanges();
        logger.LogInformation("Mailbox added to database successfully.");
    }

    public MailboxResponse UpdateMailbox(UpdateMailboxRequest mailbox)
    {
        logger.LogInformation("Updating mailbox...");
        var existingMailbox = db.Mailboxes.Find(mailbox.Id);
        if (existingMailbox == null)
        {
            throw new NotFoundException("Mailbox not found");
        }
        existingMailbox.Status = mailbox.Status;
        existingMailbox.MailWeight = mailbox.MailWeight;
        db.SaveChanges();
        logger.LogInformation("Mailbox updated successfully.");
        CreateHistory(existingMailbox);
        return mapper.Map<MailboxResponse>(existingMailbox);
    }
    
    private void CreateHistory(Mailbox mailbox)
    {
        logger.LogInformation("Adding history to database...");
        var history = new History
        {
            Status = mailbox.Status,
            Time = DateTime.Now,
            MailWeight = mailbox.MailWeight,
            IdMailbox = mailbox.Id
        };
        db.Histories.Add(history);
        db.SaveChanges();
        logger.LogInformation("History added to database successfully.");
    }
}