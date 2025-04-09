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

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Transaction? Transaction { get; set; }
}
