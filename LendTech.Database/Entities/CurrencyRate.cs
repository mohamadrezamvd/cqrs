using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت نرخ ارز
/// </summary>
public class CurrencyRate : BaseEntity
{
    public Guid FromCurrencyId { get; set; }
    public Guid ToCurrencyId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // روابط
    public virtual Currency FromCurrency { get; set; } = null!;
    public virtual Currency ToCurrency { get; set; } = null!;
}
