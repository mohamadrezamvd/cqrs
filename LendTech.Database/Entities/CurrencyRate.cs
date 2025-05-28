using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class CurrencyRate
{
    public Guid Id { get; set; }

    public Guid FromCurrencyId { get; set; }

    public Guid ToCurrencyId { get; set; }

    public decimal Rate { get; set; }

    public DateTime EffectiveDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Currency FromCurrency { get; set; } = null!;

    public virtual Currency ToCurrency { get; set; } = null!;
}
