using System;

namespace MongoDB.Driver.Extensions.Documents
{
    public abstract class DocumentBase<T>
    {
        protected DocumentBase()
        {
            CreatedOn = DateTime.UtcNow;
        }

        public T Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}