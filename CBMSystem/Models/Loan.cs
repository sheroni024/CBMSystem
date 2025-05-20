using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Loan
{
    public long LoanId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string LoanType { get; set; } = null!;

    public decimal LoanAmount { get; set; }

    public decimal InterestRate { get; set; }

    public long LoanTerm { get; set; }

    public long DurationMonths { get; set; }

    public long? StatusId { get; set; }

    public DateTime AppliedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual Status? Status { get; set; }
}
