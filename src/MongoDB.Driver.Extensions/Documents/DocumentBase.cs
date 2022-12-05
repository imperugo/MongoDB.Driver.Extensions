namespace MongoDB.Driver.Extensions.Documents;

/// <summary>
/// The document base for our collections.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DocumentBase<T>
    where T: notnull
{
    /// <summary>
    /// Initializes a new instance of the DocumentBase class.
    /// </summary>
    /// <param name="id">The document id.</param>
    protected DocumentBase(T id)
    {
        Id = id;
    }

    /// <summary>
    /// The document id.
    /// </summary>
    public T Id { get; set; }

    /// <summary>
    /// The created on (UTC).
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The modified on (UTC).
    /// </summary>
    public DateTime? ModifiedOn { get; set; }
}
