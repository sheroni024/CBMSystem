using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Status
{
    public long StatusId { get; set; }

    public string? StatusName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<SecurityAuthentication> SecurityAuthentications { get; set; } = new List<SecurityAuthentication>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
