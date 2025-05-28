namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل درخواست صفحه‌بندی
/// </summary>
public class PaginationRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// شماره صفحه
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// تعداد در هر صفحه
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// فیلد مرتب‌سازی
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// جهت مرتب‌سازی
    /// </summary>
    public bool IsDescending { get; set; }

    /// <summary>
    /// متن جستجو
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// محاسبه Skip
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;
}
