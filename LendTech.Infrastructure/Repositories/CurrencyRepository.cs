using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Database.Helpers;
using LendTech.Infrastructure.Repositories.Interfaces;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository ارزها
/// </summary>
public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
{
    private readonly CurrencyHelper _currencyHelper;

    public CurrencyRepository(LendTechDbContext context) : base(context)
    {
        _currencyHelper = new CurrencyHelper(context);
    }

    /// <inheritdoc />
    public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Currency>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeCurrencyId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.Code == code);
        
        if (excludeCurrencyId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCurrencyId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CurrencyRate?> GetRateAsync(Guid fromCurrencyId, Guid toCurrencyId, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        var effectiveDate = date ?? DateTime.UtcNow;

        return await _context.CurrencyRates
            .Where(r => r.FromCurrencyId == fromCurrencyId && 
                       r.ToCurrencyId == toCurrencyId &&
                       r.EffectiveDate <= effectiveDate &&
                       (r.ExpiryDate == null || r.ExpiryDate >= effectiveDate))
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<decimal?> GetRateByCodeAsync(string fromCode, string toCode, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        var fromCurrency = await GetByCodeAsync(fromCode, cancellationToken);
        var toCurrency = await GetByCodeAsync(toCode, cancellationToken);

        if (fromCurrency == null || toCurrency == null)
            return null;

        var rate = await GetRateAsync(fromCurrency.Id, toCurrency.Id, date, cancellationToken);
        return rate?.Rate;
    }

    /// <inheritdoc />
    public async Task<List<CurrencyRate>> GetLatestRatesAsync(Guid currencyId, CancellationToken cancellationToken = default)
    {
        var rates = await _context.CurrencyRates
            .Where(r => r.FromCurrencyId == currencyId || r.ToCurrencyId == currencyId)
            .Include(r => r.FromCurrency)
            .Include(r => r.ToCurrency)
            .OrderByDescending(r => r.EffectiveDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        return rates;
    }

    /// <inheritdoc />
    public async Task<CurrencyRate> AddRateAsync(CurrencyRate rate, CancellationToken cancellationToken = default)
    {
        await _context.CurrencyRates.AddAsync(rate, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rate;
    }

    /// <inheritdoc />
    public async Task UpdateRateAsync(CurrencyRate rate, CancellationToken cancellationToken = default)
    {
        _context.CurrencyRates.Update(rate);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<decimal> ConvertAmountAsync(decimal amount, string fromCode, string toCode, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        return await _currencyHelper.GetConvertedAmount(amount, fromCode, toCode, date);
    }
}
