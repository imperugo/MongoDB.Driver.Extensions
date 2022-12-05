namespace MongoDB.Driver.Extensions.Configurations;

/// <summary>
/// The MongoDb Configuration class
/// </summary>
public class MongoDbDatabaseConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbDatabaseConfiguration"/>.
    /// </summary>
    /// <param name="connectionString"></param>
    public MongoDbDatabaseConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// The database connection string.
    /// </summary>
    public string ConnectionString { get; set; }
}
