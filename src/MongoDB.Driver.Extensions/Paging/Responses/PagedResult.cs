using System.Collections.Generic;

namespace MongoDB.Driver.Extensions.Paging.Responses
{
	/// <summary>
	///     The implementation of <see cref="IPagedResult{T}" />
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public class PagedResult<T> : IPagedResult<T>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="PagedResult{T}" /> class.
		/// </summary>
		public PagedResult()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="PagedResult&lt;T&gt;" /> class.
		/// </summary>
		/// <param name="pageIndex"> Index of the page. </param>
		/// <param name="pageSize"> Size of the page. </param>
		/// <param name="result"> The result. </param>
		/// <param name="totalCount"> The total count. </param>
		public PagedResult(int pageIndex, int pageSize, IEnumerable<T> result, long totalCount)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			Result = result;
			TotalCount = totalCount;
		}

		/// <summary>
		///     Gets a value indicating whether this instance has next page.
		/// </summary>
		/// <value> <c>true</c> if this instance has next page; otherwise, <c>false</c> . </value>
		public bool HasNextPage => PageIndex + 1 < TotalPages;

		/// <summary>
		///     Gets a value indicating whether this instance has previous page.
		/// </summary>
		/// <value> <c>true</c> if this instance has previous page; otherwise, <c>false</c> . </value>
		public bool HasPreviousPage => PageIndex > 0;

		/// <summary>
		///     Gets or sets the index of the page.
		/// </summary>
		/// <value> The index of the page. </value>
		public int PageIndex { get; set; }

		/// <summary>
		///     Gets or sets the size of the page.
		/// </summary>
		/// <value> The size of the page. </value>
		public int PageSize { get; set; }

		/// <summary>
		///     Gets or sets the result.
		/// </summary>
		/// <value> The result. </value>
		public IEnumerable<T> Result { get; set; }

		/// <summary>
		///     Gets or sets the total count.
		/// </summary>
		/// <value> The total count. </value>
		public long TotalCount { get; set; }

		/// <summary>
		///     Gets the total pages.
		/// </summary>
		/// <value> The total pages. </value>
		public int TotalPages
		{
			get
			{
				if (Result == null)
				{
					return 0;
				}

				return (int) (TotalCount / PageSize) + (TotalCount % PageSize == 0
					       ? 0
					       : 1);
			}
		}
	}
}