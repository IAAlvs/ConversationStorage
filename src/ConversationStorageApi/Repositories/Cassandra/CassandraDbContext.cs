/* using Cassandra;

public class CassandraDbContext : ICassandraDbContext
{
    private readonly Cluster _cluster;
    private readonly Cassandra.ISession _session;
    private readonly IConfiguration? _config;


    public CassandraDbContext(IConfiguration _config)
    {
        const string defaultKeySpace = "conversations";
        string connectionString = _config.GetConnectionString("CASSANDRA_DB")??
            throw new Exception("NOT DB CONFIG PROVIDED");
        Console.WriteLine("Connection : " + connectionString);
        _cluster = Cluster.Builder()
            .AddContactPoint(connectionString)
            .Build();

        _session = _cluster.Connect(defaultKeySpace);
    }

    public Cassandra.ISession Session => _session;
}
 */