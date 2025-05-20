using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class DematAccount
{
    public long DematId { get; set; }

    public long CustomerId { get; set; }

    public string? BrokerName { get; set; }

    public string? AccountNumber { get; set; }

    public bool? AadhaarLinked { get; set; }

    public bool? Panlinked { get; set; }

    public decimal? Balance { get; set; }

    public string? AccountStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public virtual CustomerAccount Customer { get; set; } = null!;
}
