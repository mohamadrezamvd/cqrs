using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database.Entities;

namespace LendTech.Database.Helpers;

/// <summary>
/// کلاس کمکی برای عملیات مربوط به ارز
/// </summary>
public class CurrencyHelper
{
    private readonly LendTechDbContext _context;

    public CurrencyHelper(LendTechDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// تبدیل مبلغ از یک ارز به ارز دیگر
    /// </summary>
    /// <param name="amount">مبلغ</param>
    /// <param name="fromCurrencyCode">کد ارز مبدا</param>
    /// <param name="toCurrencyCode">کد ارز مقصد</param>
    /// <param name="date">تاریخ نرخ (پیش‌فرض: امروز)</param>
    /// <returns>مبلغ تبدیل شده</returns>
    public async Task<decimal> GetConvertedAmount(
        decimal amount, 
        string fromCurrencyCode, 
        string toCurrencyCode,
        DateTime? date = null)
    {
        // اگر ارز مبدا و مقصد یکی باشند
        if (fromCurrencyCode.Equals(toCurrencyCode, StringComparison.OrdinalIgnoreCase))
            return amount;

        var effectiveDate = date ?? DateTime.UtcNow;

        // پیدا کردن ارزها
        var fromCurrency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.Code == fromCurrencyCode && c.IsActive);

        var toCurrency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.Code == toCurrencyCode && c.IsActive);

        if (fromCurrency == null || toCurrency == null)
            throw new ArgumentException("ارز مورد نظر یافت نشد یا غیرفعال است");

        // پیدا کردن نرخ مستقیم
        var directRate = await _context.CurrencyRates
            .Where(r => r.FromCurrencyId == fromCurrency.Id 
                     && r.ToCurrencyId == toCurrency.Id
                     && r.EffectiveDate <= effectiveDate
                     && (r.ExpiryDate == null || r.ExpiryDate >= effectiveDate))
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (directRate != null)
            return amount * directRate.Rate;

        // پیدا کردن نرخ معکوس
        var reverseRate = await _context.CurrencyRates
            .Where(r => r.FromCurrencyId == toCurrency.Id 
                     && r.ToCurrencyId == fromCurrency.Id
                     && r.EffectiveDate <= effectiveDate
                     && (r.ExpiryDate == null || r.ExpiryDate >= effectiveDate))
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (reverseRate != null)
            return amount / reverseRate.Rate;

        // پیدا کردن نرخ از طریق ارز واسط (USD)
        var usdCurrency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.Code == "USD" && c.IsActive);

        if (usdCurrency != null && 
            fromCurrency.Id != usdCurrency.Id && 
            toCurrency.Id != usdCurrency.Id)
        {
            var fromToUsdRate = await GetRate(fromCurrency.Id, usdCurrency.Id, effectiveDate);
            var usdToToRate = await GetRate(usdCurrency.Id, toCurrency.Id, effectiveDate);

            if (fromToUsdRate.HasValue && usdToToRate.HasValue)
                return amount * fromToUsdRate.Value * usdToToRate.Value;
        }

        throw new InvalidOperationException(
            $"نرخ تبدیل از {fromCurrencyCode} به {toCurrencyCode} برای تاریخ {effectiveDate:yyyy-MM-dd} یافت نشد");
    }

    /// <summary>
    /// دریافت نرخ ارز
    /// </summary>
    private async Task<decimal?> GetRate(Guid fromCurrencyId, Guid toCurrencyId, DateTime date)
    {
        var rate = await _context.CurrencyRates
            .Where(r => r.FromCurrencyId == fromCurrencyId 
                     && r.ToCurrencyId == toCurrencyId
                     && r.EffectiveDate <= date
                     && (r.ExpiryDate == null || r.ExpiryDate >= date))
            .OrderByDescending(r => r.EffectiveDate)
            .Select(r => r.Rate)
            .FirstOrDefaultAsync();

        return rate == 0 ? null : rate;
    }
}
