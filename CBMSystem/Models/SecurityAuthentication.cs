using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class SecurityAuthentication
{
    public long LogId { get; set; }

    public long? UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public string? Ipaddress { get; set; }

    public long? StatusId { get; set; }

    public virtual Status? Status { get; set; }

    public virtual User? User { get; set; }
}
