using System.ComponentModel.DataAnnotations;

namespace MongoDB.Driver.Extensions.Paging.Requests
{
	public class SimpleKeyRequest<T>
	{
		[Required]
		public virtual T Id { get; set; }
	}
}