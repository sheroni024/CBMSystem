using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Authentication
{
    public long AuthId { get; set; }

    public long? UserId { get; set; }

    public string? Otp { get; set; }

    public DateTime? Otpexpiry { get; set; }

    public long? LoginAttempts { get; set; }

    public DateTime? LastAttempt { get; set; }

    public virtual User? User { get; set; }
}
