using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Employee
{
    public long EmployeeId { get; set; }

    public string? FullName { get; set; }

    public string? LastName { get; set; }

    public string? Designation { get; set; }

    public string? Departmemt { get; set; }

    public long? BranchCodeId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public virtual Branch? BranchCode { get; set; }
}
