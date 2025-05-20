using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class FraudReport
{
    public long FraudReportId { get; set; }

    public long CustomerId { get; set; }

    public string? AccountNumber { get; set; }

    public long FraudTypeId { get; set; }

    public string? Description { get; set; }

    public DateTime ReportedDate { get; set; }

    public string? Status { get; set; }

    public virtual CustomerAccount Customer { get; set; } = null!;

    public virtual ICollection<FraudAction> FraudActions { get; set; } = new List<FraudAction>();

    public virtual FraudType FraudType { get; set; } = null!;
}
