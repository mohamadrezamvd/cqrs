using System;
using System.Collections.Generic;

namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل نتیجه صفحه‌بندی شده
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// لیست آیتم‌ها
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// تعداد کل
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// شماره صفحه جاری
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// تعداد در هر صفحه
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// تعداد کل صفحات
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// آیا صفحه قبلی دارد
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// آیا صفحه بعدی دارد
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// سازنده
    /// </summary>
    public PagedResult()
    {
    }

    /// <summary>
    /// سازنده با پارامتر
    /// </summary>
    public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
