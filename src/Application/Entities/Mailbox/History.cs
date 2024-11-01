using System;
using System.Collections.Generic;

namespace API;

public partial class History
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? Time { get; set; }

    public double MailWeight { get; set; }

    public int IdMailbox { get; set; }

    public virtual Mailbox IdMailboxNavigation { get; set; } = null!;
}
