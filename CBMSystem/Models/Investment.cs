using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Investment
{
    public long InvestmentId { get; set; }

    public long CustomerId { get; set; }

    public long DematId { get; set; }

    public string InvestmentType { get; set; } = null!;

    public string InvestmentName { get; set; } = null!;

    public decimal InvestmentAmount { get; set; }

    public decimal? UnitsPurchased { get; set; }

    public decimal? Nav { get; set; }

    public decimal? StockPrice { get; set; }

    public string? PolicyNumber { get; set; }

    public long? SchemeId { get; set; }

    public DateTime? InvestmentDate { get; set; }

    public DateTime? RedemptionDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
