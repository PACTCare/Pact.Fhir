namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Services;

  /// <summary>
  /// Basic example implementation of the resource tracker.
  /// Yes, the SQL could be optimized :-)
  /// </summary>
  public class SqlLiteResourceTracker : IResourceTracker
  {
    public SqlLiteResourceTracker(
      MamChannelFactory channelFactory,
      MamChannelSubscriptionFactory subscriptionFactory,
      string databaseFilename = "resourcetracker.sqlite")
    {
      this.ChannelFactory = channelFactory;
      this.SubscriptionFactory = subscriptionFactory;
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      if (!File.Exists(databaseFilename))
      {
        this.InitDatabase(databaseFilename);
      }
    }

    private MamChannelFactory ChannelFactory { get; }

    private string ConnectionString { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task AddEntryAsync(ResourceEntry entry)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();
        long resourceId;

        using (var command = new SQLiteCommand(
          $"INSERT INTO Resource (Channel, Subscription) VALUES ('{entry.Channel.ToJson()}', '{entry.Subscription.ToJson()}'); SELECT last_insert_rowid();",
          connection))
        {
          resourceId = (long)await command.ExecuteScalarAsync();
        }

        foreach (var hash in entry.StreamHashes)
        {
          using (var command = new SQLiteCommand($"INSERT INTO StreamHash (Hash, ResourceId) VALUES ('{hash.Value}', '{resourceId}')", connection))
          {
            await command.ExecuteScalarAsync();
          }
        }
      }
    }

    /// <inheritdoc />
    public async Task<ResourceEntry> GetEntryAsync(string versionId)
    {
      ResourceEntry entry = null;
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE Hash = '{versionId}'", connection))
        {
          var result = await command.ExecuteReaderAsync();
          if (result.Read())
          {
            resourceId = (long)result["ResourceId"];
          }
          else
          {
            return null;
          }
        }

        var streamHashes = new List<Hash>();
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE ResourceId = '{resourceId}'", connection))
        {
          var result = await command.ExecuteReaderAsync();
          while (result.Read())
          {
            streamHashes.Add(new Hash(result["Hash"].ToString()));
          }
        }

        using (var command = new SQLiteCommand($"SELECT * FROM Resource WHERE Id = '{resourceId}'", connection))
        {
          var result = await command.ExecuteReaderAsync();
          if (result.Read())
          {
            entry = new ResourceEntry
                     {
                       StreamHashes = streamHashes,
                       Channel = this.ChannelFactory.CreateFromJson(result["Channel"].ToString()),
                       Subscription = this.SubscriptionFactory.CreateFromJson(result["Subscription"].ToString())
                     };
          }
        }
      }

      return entry;
    }

    private void InitDatabase(string databaseFilename)
    {
      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand(
          "CREATE TABLE Resource (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Channel TEXT NULL, Subscription TEXT NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "CREATE TABLE StreamHash (Hash VARCHAR(81) NOT NULL PRIMARY KEY, ResourceId INTEGER NOT NULL, FOREIGN KEY (ResourceId) REFERENCES Resource(Id))",
          connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}