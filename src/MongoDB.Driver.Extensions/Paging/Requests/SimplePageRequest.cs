using System.ComponentModel.DataAnnotations;

namespace MongoDB.Driver.Extensions.Paging.Requests
{
	public class SimplePagedRequest
	{
		public SimplePagedRequest()
		{
			PageIndex = 0;
			PageSize = 10;
		}

		/// <summary>
		///     Gets or sets the index of the page.
		/// </summary>
		/// <value>
		///     The index of the page.
		/// </value>
		[Range(0, int.MaxValue)]
		public int PageIndex { get; set; }

		/// <summary>
		///     Gets or sets the size of the page.
		/// </summary>
		/// <value>
		///     The size of the page.
		/// </value>
		[Range(1, int.MaxValue)]
		public int PageSize { get; set; }
	}
}