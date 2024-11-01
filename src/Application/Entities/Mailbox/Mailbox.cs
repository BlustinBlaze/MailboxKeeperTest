using System;
using System.Collections.Generic;

namespace API;

public partial class Mailbox
{
    public int Id { get; set; }

    public string Password { get; set; } = null!;

    public string Status { get; set; } = null!;

    public double MailWeight { get; set; }

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
