using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class FraudType
{
    public long FraudTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<FraudReport> FraudReports { get; set; } = new List<FraudReport>();
}
