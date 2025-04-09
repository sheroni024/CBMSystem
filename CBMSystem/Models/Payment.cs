using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Payment
{
    public long PaymentId { get; set; }

    public long? AccountId { get; set; }

    public string? PayeeName { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    public long? StatusId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Status? Status { get; set; }
}
