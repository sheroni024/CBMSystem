using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Branch
{
    public long BranchCodeId { get; set; }

    public string? BranchName { get; set; }

    public string? Address { get; set; }

    public string? ContactNumber { get; set; }

    public string? BranchManager { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
