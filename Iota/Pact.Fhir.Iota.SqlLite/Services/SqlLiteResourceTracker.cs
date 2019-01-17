namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;

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
      IEncryption encryption,
      string databaseFilename = "resourcetracker.sqlite")
    {
      this.ChannelFactory = channelFactory;
      this.SubscriptionFactory = subscriptionFactory;
      this.Encryption = encryption;
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      if (!File.Exists(databaseFilename))
      {
        this.InitDatabase(databaseFilename);
      }
    }

    public IEncryption Encryption { get; }

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

        var encryptedChannel = this.Encryption.Encrypt(entry.Channel.ToJson());
        var encryptedSubscription = this.Encryption.Encrypt(entry.Subscription.ToJson());

        using (var command = new SQLiteCommand(
          $"INSERT INTO Resource (Channel, Subscription) VALUES ('{encryptedChannel}', '{encryptedSubscription}'); SELECT last_insert_rowid();",
          connection))
        {
          resourceId = (long)await command.ExecuteScalarAsync();
        }

        foreach (var id in entry.ResourceIds)
        {
          using (var command = new SQLiteCommand(
            $"INSERT INTO StreamHash (Hash, ResourceId) VALUES ('{this.Encryption.Encrypt(id)}', '{resourceId}')",
            connection))
          {
            await command.ExecuteNonQueryAsync();
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
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE Hash = '{this.Encryption.Encrypt(versionId)}'", connection))
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

        var resourceIds = new List<string>();
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE ResourceId = '{resourceId}'", connection))
        {
          var result = await command.ExecuteReaderAsync();
          while (result.Read())
          {
            var decryptedHash = this.Encryption.Decrypt(result["Hash"].ToString());

            resourceIds.Add(decryptedHash);
          }
        }

        using (var command = new SQLiteCommand($"SELECT * FROM Resource WHERE Id = '{resourceId}'", connection))
        {
          var result = await command.ExecuteReaderAsync();
          if (result.Read())
          {
            var decryptedChannel = this.Encryption.Decrypt(result["Channel"].ToString());
            var decryptedSubscription = this.Encryption.Decrypt(result["Subscription"].ToString());

            entry = new ResourceEntry
                      {
                        ResourceIds = resourceIds,
                        Channel = this.ChannelFactory.CreateFromJson(decryptedChannel),
                        Subscription = this.SubscriptionFactory.CreateFromJson(decryptedSubscription)
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