using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class Currency
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Symbol { get; set; }

    public int DecimalPlaces { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<CurrencyRate> CurrencyRateFromCurrencies { get; set; } = new List<CurrencyRate>();

    public virtual ICollection<CurrencyRate> CurrencyRateToCurrencies { get; set; } = new List<CurrencyRate>();
}
