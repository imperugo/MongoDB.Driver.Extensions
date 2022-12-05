namespace MongoDB.Driver.Extensions.Paging.Requests;

/// <summary>
/// The query pagination request.
/// </summary>
public class SimplePagedRequest
{
    /// <summary>
    /// Initializes a new instance of the SimplePagedRequest class.
    /// </summary>
    public SimplePagedRequest()
    {
        PageIndex = 0;
        PageSize = 10;
    }

    /// <summary>
    /// Initializes a new instance of the SimplePagedRequest class.
    /// </summary>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="pageSize">The page size.</param>
    public SimplePagedRequest(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    ///     Gets or sets the index of the page.
    /// </summary>
    /// <value>
    ///     The index of the page.
    /// </value>
    public int PageIndex { get; set; }

    /// <summary>
    ///     Gets or sets the size of the page.
    /// </summary>
    /// <value>
    ///     The size of the page.
    /// </value>
    public int PageSize { get; set; }
}
