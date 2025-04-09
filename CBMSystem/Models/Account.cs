using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Account
{
    public long AccountId { get; set; }

    public long CustomerId { get; set; }

    public long? BranchCodeId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public decimal Balance { get; set; }

    public string? Currency { get; set; }

    public long? StatusId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();

    public virtual Branch? BranchCode { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual CustomerAccount Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Status? Status { get; set; }
}
