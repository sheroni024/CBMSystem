using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class FraudAction
{
    public long FraudActionId { get; set; }

    public long FraudReportId { get; set; }

    public string? ActionTaken { get; set; }

    public DateTime? ActionDate { get; set; }

    public long? PerformedBy { get; set; }

    public virtual FraudReport FraudReport { get; set; } = null!;

    public virtual User? PerformedByNavigation { get; set; }
}
