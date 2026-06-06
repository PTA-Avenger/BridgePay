namespace BridgePay.Infrastructure.Persistence.MongoDB;

/// <summary>
/// Configuration settings for connecting to MongoDB.
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// Gets or sets the MongoDB connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;
}
