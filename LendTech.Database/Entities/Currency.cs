using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت ارز
/// </summary>
public class Currency : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Symbol { get; set; }
    public int DecimalPlaces { get; set; }
    public bool IsActive { get; set; }

    // روابط
    public virtual ICollection<CurrencyRate> FromCurrencyRates { get; set; } = new List<CurrencyRate>();
    public virtual ICollection<CurrencyRate> ToCurrencyRates { get; set; } = new List<CurrencyRate>();
}
