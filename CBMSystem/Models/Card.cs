using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class Card
{
    public long CardId { get; set; }

    public long? AccountId { get; set; }

    public string? CardNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? Cvv { get; set; }

    public string? CardType { get; set; }

    public long? StatusId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Status? Status { get; set; }
}
