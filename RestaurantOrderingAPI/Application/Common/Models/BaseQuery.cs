using System;

namespace RestaurantOrderingAPI.Application.Common.Models;

public class BaseQuery(int maxPageSize)
{
    public bool Ascending { get; set; } = true;
    public string? SortBy { get; set; }
    public string? Search { get; set; }

    //Pagination
    public int PageNumber { get; set; } = 1;
    private int _pageSize;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Min(value, maxPageSize);
    }
}
