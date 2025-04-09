using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Notification
{
    public long NotificationId { get; set; }

    public long? UserId { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsRead { get; set; }

    public virtual User? User { get; set; }
}
