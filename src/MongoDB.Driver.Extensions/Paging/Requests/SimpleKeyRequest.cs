namespace MongoDB.Driver.Extensions.Paging.Requests
{
	public class SimpleKeyRequest<T>
	{
        public SimpleKeyRequest()
        {
        }

        public SimpleKeyRequest(T id)
        {
            Id = id;
        }

        public virtual T Id { get; set; }
	}
}