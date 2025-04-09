using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Role
{
    public long RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? Permissions { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }
}
