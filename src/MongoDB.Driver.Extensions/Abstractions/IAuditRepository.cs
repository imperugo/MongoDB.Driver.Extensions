namespace MongoDB.Driver.Extensions.Abstractions;

/// <summary>
/// The contract for the implementation of the Audit repository.
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Checks if the connection to the MongoDb instance is ok.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The status of the connection.</returns>
    Task<DbStatus> CheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// The database connection status
/// </summary>
public class DbStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbStatus"/> class.
    /// </summary>
    /// <param name="dbName">The name of the database used to verify the connection.</param>
    /// <param name="running"><c>True</c> if the connection is ok, otherwise <c>False</c>.</param>
    public DbStatus(string dbName, bool running)
    {
        DbName = dbName;
        Running = running;
    }

    /// <summary>
    /// The name of the database used to verify the connection.
    /// </summary>
    public string DbName { get; }

    /// <summary>
    /// <c>True</c> if the connection is ok, otherwise <c>False</c>.
    /// </summary>
    public bool Running { get; }
}
