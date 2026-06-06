namespace BridgePay.Application.Common.DTOs;

using System.Collections.Generic;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
public class PagedResultDto<T>
{
    /// <summary>
    /// Gets or sets the items in the current page.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Gets or sets the total number of items matching the filters.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the current page number (1-indexed).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}
