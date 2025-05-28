using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository ارزها
/// </summary>
public interface ICurrencyRepository : IRepository<Currency>
{
    /// <summary>
    /// دریافت ارز با کد
    /// </summary>
    Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت ارزهای فعال
    /// </summary>
    Task<List<Currency>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود کد ارز
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeCurrencyId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نرخ ارز
    /// </summary>
    Task<CurrencyRate?> GetRateAsync(Guid fromCurrencyId, Guid toCurrencyId, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نرخ ارز با کد
    /// </summary>
    Task<decimal?> GetRateByCodeAsync(string fromCode, string toCode, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت آخرین نرخ‌های یک ارز
    /// </summary>
    Task<List<CurrencyRate>> GetLatestRatesAsync(Guid currencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ثبت نرخ ارز جدید
    /// </summary>
    Task<CurrencyRate> AddRateAsync(CurrencyRate rate, CancellationToken cancellationToken = default);

    /// <summary>
    /// به‌روزرسانی نرخ ارز
    /// </summary>
    Task UpdateRateAsync(CurrencyRate rate, CancellationToken cancellationToken = default);

    /// <summary>
    /// تبدیل مبلغ
    /// </summary>
    Task<decimal> ConvertAmountAsync(decimal amount, string fromCode, string toCode, DateTime? date = null, CancellationToken cancellationToken = default);
}
