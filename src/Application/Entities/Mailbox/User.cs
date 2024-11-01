using System;
using System.Collections.Generic;

namespace API;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Notification { get; set; }

    public int IdMailbox { get; set; }

    public virtual Mailbox IdMailboxNavigation { get; set; } = null!;
}
