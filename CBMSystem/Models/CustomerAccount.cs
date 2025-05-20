using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class CustomerAccount
{
    public long CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? AccountNumber { get; set; }

    public string Email { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string PancardNumber { get; set; } = null!;

    public string AadhaarNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public string? ProfileImage { get; set; }

    public string? SignatureImage { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<DematAccount> DematAccounts { get; set; } = new List<DematAccount>();

    public virtual ICollection<FraudReport> FraudReports { get; set; } = new List<FraudReport>();
}
