namespace LendTech.Application.DTOs.Common;
/// <summary>
/// DTO درخواست صفحه‌بندی
/// </summary>
public class PagedRequestDto
{
private int _pageNumber = 1;
private int _pageSize = 10;
public int PageNumber
{
    get => _pageNumber;
    set => _pageNumber = value < 1 ? 1 : value;
}

public int PageSize
{
    get => _pageSize;
    set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
}

public string? SortBy { get; set; }
public bool IsDescending { get; set; }
public string? SearchTerm { get; set; }
public Dictionary<string, string>? Filters { get; set; }
}
