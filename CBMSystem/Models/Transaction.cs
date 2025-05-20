using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Transaction
{
    public long TransactionId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string TransactionType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string ReferenceNumber { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }

    public decimal BalanceAfterTransaction { get; set; }

    public long BranchCodeId { get; set; }

    public long? StatusId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Account AccountNumberNavigation { get; set; } = null!;

    public virtual Branch BranchCode { get; set; } = null!;

    public virtual Status? Status { get; set; }
}
