using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس پایه Repository
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// دریافت تمام موجودیت‌ها به صورت IQueryable
    /// </summary>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// دریافت موجودیت با شناسه
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت موجودیت با شناسه به همراه Include
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// دریافت اولین موجودیت با شرط
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت اولین موجودیت با شرط و Include
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// دریافت تمام موجودیت‌ها
    /// </summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت موجودیت‌ها با شرط
    /// </summary>
    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت موجودیت‌ها با شرط و Include
    /// </summary>
    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// دریافت موجودیت‌ها با صفحه‌بندی
    /// </summary>
    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isDescending = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود موجودیت با شرط
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// شمارش موجودیت‌ها
    /// </summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزودن موجودیت
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزودن چند موجودیت
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// به‌روزرسانی موجودیت
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// به‌روزرسانی چند موجودیت
    /// </summary>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف موجودیت
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف موجودیت با شناسه
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف چند موجودیت
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// اجرای SQL خام
    /// </summary>
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

    /// <summary>
    /// اجرای SQL خام با نتیجه
    /// </summary>
    Task<List<TEntity>> FromSqlRawAsync(string sql, params object[] parameters);
}
